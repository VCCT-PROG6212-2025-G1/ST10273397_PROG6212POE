using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using PROG6212POE.Controllers;
using PROG6212POE.Models;
using PROG6212POE.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static PROG6212POE.Models.UserModel;

namespace PROG6212POE_Testing
{
    [TestClass]
    public class ControllersTests
    {
        private AppDbContext _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Seed test users
            _context.UserModel.AddRange(
                new UserModel
                {
                    UserId = 1,
                    FirstName = "Jay",
                    LastName = "Doe",
                    Email = "jay.doe@test.com",
                    Password = "Password123!",
                    UserRole = Role.Lecturer,
                    HourlyRate = 350.0f
                },
                new UserModel
                {
                    UserId = 2,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane.doe@test.com",
                    Password = "Password123!",
                    UserRole = Role.ProgCoord,
                    HourlyRate = 450.0f
                },
                new UserModel
                {
                    UserId = 3,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@test.com",
                    Password = "Password123!",
                    UserRole = Role.AcadMan,
                    HourlyRate = 500.0f
                }
            );
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Helpers

        // Minimal in-memory ISession implementation for tests
        private class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _store = new Dictionary<string, byte[]>();

            public IEnumerable<string> Keys => _store.Keys;
            public string Id { get; } = Guid.NewGuid().ToString();
            public bool IsAvailable => true;

            public void Clear() => _store.Clear();
            public Task CommitAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task LoadAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) => _store.Remove(key);

            public void Set(string key, byte[] value) => _store[key] = value;
            public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value);
        }

        // Small ISessionFeature holder so DefaultHttpContext.Session works
        private class TestSessionFeature : ISessionFeature
        {
            public ISession Session { get; set; }
        }

        // Instantiate controller and attach HttpContext with a test session
        private T CreateController<T>(TestSession session = null) where T : Controller
        {
            session ??= new TestSession();

            // Create instance via reflection (assumes constructor (AppDbContext) exists)
            var controller = (T)Activator.CreateInstance(typeof(T), _context);

            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(new TestSessionFeature { Session = session });

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        // Helper to set integer session value (stored as 4-byte little-endian)
        private void SetSessionInt(ISession session, string key, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            session.Set(key, bytes);
        }

        private int? GetSessionInt(ISession session, string key)
        {
            if (session.TryGetValue(key, out var bytes) && bytes?.Length >= 4 == true)
                return BitConverter.ToInt32(bytes, 0);
            return null;
        }

        #endregion

        #region LoginController Tests

        [TestMethod]
        public void LoginController_GET_ReturnsView()
        {
            var controller = CreateController<LoginController>();
            var result = controller.Login() as ViewResult;
            Assert.IsNotNull(result);
        }

        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("invalid@test.com", "Password123!")]
        [DataRow("jay.doe@test.com", "WrongPassword")]
        public void LoginController_POST_InvalidCredentials_ReturnsViewWithModelError(string email, string password)
        {
            var controller = CreateController<LoginController>();
            var result = controller.Login(email, password) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid, "ModelState should be invalid for bad credentials");
        }

        [TestMethod]
        public void LoginController_POST_ValidLecturer_RedirectsToOverview()
        {
            var session = new TestSession();
            var controller = CreateController<LoginController>(session);

            var result = controller.Login("jay.doe@test.com", "Password123!") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Overview", result.ActionName);
            Assert.AreEqual("Lecturer", result.ControllerName);

            // Optionally check session was set (if controller sets UserId or UserRole)
            // Example check: if controller sets "UserRole" string in session
            if (session.TryGetValue("UserRole", out var roleBytes))
            {
                var roleString = System.Text.Encoding.UTF8.GetString(roleBytes);
                Assert.IsTrue(roleString.Contains("Lecturer"));
            }
        }

        [TestMethod]
        public void LoginController_POST_ValidProgrammeCoordinator_RedirectsToClaimList()
        {
            var session = new TestSession();
            var controller = CreateController<LoginController>(session);

            var result = controller.Login("jane.doe@test.com", "Password123!") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("PCClaimList", result.ActionName);
            Assert.AreEqual("PCAM", result.ControllerName);
        }

        [TestMethod]
        public void LoginController_POST_ValidAcademicManager_RedirectsToAMClaimList()
        {
            var session = new TestSession();
            var controller = CreateController<LoginController>(session);

            var result = controller.Login("john.smith@test.com", "Password123!") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("AMClaimList", result.ActionName);
            Assert.AreEqual("PCAM", result.ControllerName);
        }

        #endregion

        #region LecturerController Tests

        [TestMethod]
        public void LecturerController_SubmitClaim_GET_ReturnsView()
        {
            var session = new TestSession();
            SetSessionInt(session, "UserId", 1);

            var controller = CreateController<LecturerController>(session);

            var result = controller.SubmitClaim() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ClaimModel));
        }

        [TestMethod]
        public async Task LecturerController_SubmitClaim_POST_ValidClaim_Saves()
        {
            var session = new TestSession();
            SetSessionInt(session, "UserId", 1);

            var controller = CreateController<LecturerController>(session);

            var claim = new ClaimModel
            {
                UserId = 1,
                UserName = "Jay Doe",
                Title = "Test Claim",
                HoursWorked = 10,
                HourlyRate = 350.0f,
                AdditionalNotes = "Test Note"
            };

            var result = await controller.SubmitClaim(claim, null) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Overview", result.ActionName);

            var savedClaim = _context.ClaimModel.FirstOrDefault();
            Assert.IsNotNull(savedClaim);
            Assert.AreEqual("Pending", savedClaim.ClaimStatus);
            Assert.AreEqual("Test Claim", savedClaim.Title);
            Assert.AreEqual(1, savedClaim.UserId);
        }

        [TestMethod]
        public async Task LecturerController_SubmitClaim_POST_InvalidFileType_ReturnsViewWithError()
        {
            // build a fake IFormFile with PNG type to simulate invalid type if controller rejects it
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("image/png");
            fileMock.Setup(f => f.FileName).Returns("test.png");

            var session = new TestSession();
            SetSessionInt(session, "UserId", 1);
            var controller = CreateController<LecturerController>(session);

            var claim = new ClaimModel
            {
                UserId = 1,
                UserName = "Jay Doe",
                Title = "File Test",
                HoursWorked = 5,
                HourlyRate = 350.0f
            };

            var result = await controller.SubmitClaim(claim, fileMock.Object) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState.Keys.Any(k => k.Contains("SuppDoc") || k.Contains("SuppDocName")));
        }

        [TestMethod]
        public async Task LecturerController_SubmitClaim_POST_FileTooLarge_ReturnsViewWithError()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(15 * 1024 * 1024); // 15 MB
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            fileMock.Setup(f => f.FileName).Returns("test.pdf");

            var session = new TestSession();
            SetSessionInt(session, "UserId", 1);
            var controller = CreateController<LecturerController>(session);

            var claim = new ClaimModel
            {
                UserId = 1,
                UserName = "Jay Doe",
                Title = "File Test",
                HoursWorked = 5,
                HourlyRate = 350.0f
            };

            var result = await controller.SubmitClaim(claim, fileMock.Object) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public void LecturerController_Overview_ReturnsViewWithUserClaims()
        {
            // Add test claim
            _context.ClaimModel.Add(new ClaimModel
            {
                ClaimId = 1,
                UserId = 1,
                UserName = "Jay Doe",
                Title = "Test Claim",
                HoursWorked = 10,
                HourlyRate = 350.0f,
                ClaimStatus = "Pending"
            });
            _context.SaveChanges();

            var session = new TestSession();
            SetSessionInt(session, "UserId", 1);
            var controller = CreateController<LecturerController>(session);

            var result = controller.Overview() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ClaimModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(1, model[0].UserId);
        }

        #endregion

        #region HRController Tests

        [TestMethod]
        public void HRController_Index_ReturnsViewWithUsersAndClaims()
        {
            var controller = CreateController<HRController>();
            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as List<UserModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Count);
        }

        [TestMethod]
        public void HRController_Details_ReturnsViewWithUserAndClaims()
        {
            // Add test claim for user 1
            _context.ClaimModel.Add(new ClaimModel
            {
                ClaimId = 1,
                UserId = 1,
                UserName = "Jay Doe",
                Title = "Test Claim",
                HoursWorked = 10,
                HourlyRate = 350.0f,
                ClaimStatus = "Pending"
            });
            _context.SaveChanges();

            var controller = CreateController<HRController>();
            var result = controller.Details(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(UserModel));

            var claims = result.ViewData["Claims"] as List<ClaimModel>;
            Assert.IsNotNull(claims);
            Assert.AreEqual(1, claims.Count);
            Assert.AreEqual(1, claims[0].UserId);
        }

        [TestMethod]
        public async Task HRController_CreateUser_POST_ValidUser_Saves()
        {
            var controller = CreateController<HRController>();
            var newUser = new UserModel
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test.user@test.com",
                Password = "TestPass123",
                UserRole = Role.Lecturer,
                HourlyRate = 300.0f
            };

            var result = await controller.CreateUser(newUser) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var savedUser = _context.UserModel.FirstOrDefault(u => u.Email == "test.user@test.com");
            Assert.IsNotNull(savedUser);
            Assert.AreEqual("Test", savedUser.FirstName);
        }

        [TestMethod]
        public async Task HRController_Edit_POST_ValidUser_Updates()
        {
            var controller = CreateController<HRController>();
            var updatedUser = new UserModel
            {
                UserId = 1,
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@test.com",
                Password = "NewPass123",
                UserRole = Role.Lecturer,
                HourlyRate = 400.0f
            };

            var result = await controller.Edit(updatedUser) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var user = _context.UserModel.Find(1);
            Assert.AreEqual("Updated", user.FirstName);
            Assert.AreEqual("Name", user.LastName);
            Assert.AreEqual(400.0f, user.HourlyRate);
        }

        #endregion

        #region PCAMController Tests (optional)

        [TestMethod]
        public void PCAMController_ClaimList_SmokeTest()
        {
            // Seed a claim
            _context.ClaimModel.Add(new ClaimModel
            {
                ClaimId = 1,
                UserId = 1,
                UserName = "Jay Doe",
                Title = "Claim 1",
                HoursWorked = 5,
                HourlyRate = 350.0f,
                ClaimStatus = "Pending"
            });
            _context.SaveChanges();

            // If you have a PCAMController in your project, uncomment and use:
            // var controller = CreateController<PCAMController>();
            // var result = controller.ClaimList() as ViewResult;
            // Assert.IsNotNull(result);
            // var model = result.Model as List<ClaimModel>;
            // Assert.AreEqual(1, model.Count);
        }

        #endregion
    }
}
