using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using PROG6212POE.Controllers;
using PROG6212POE.Models;
using PROG6212POE.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
        }

        #region LoginController Tests

        [TestMethod]
        public void LoginController_GET_ReturnsView()
        {
            var controller = new LoginController();
            var result = controller.Login() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginController_POST_Invalid_ReturnsViewWithError()
        {
            var controller = new LoginController();
            var result = controller.Login("", "", "") as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void LoginController_POST_ValidLecturer_RedirectsToOverview()
        {
            var controller = new LoginController();
            var contextMock = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = contextMock };

            var result = controller.Login("JayDoe", "Password123!", "Lecturer") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Overview", result.ActionName);
            Assert.AreEqual("Lecturer", contextMock.Session.GetString("UserRole"));
        }

        [TestMethod]
        public void LoginController_POST_ValidProgrammeCoordinator_RedirectsToClaimList()
        {
            var controller = new LoginController();
            var contextMock = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = contextMock };

            var result = controller.Login("JaneDoe", "Password123!", "ProgrammeCoordinator") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ClaimList", result.ActionName);
            Assert.AreEqual("ProgrammeCoordinator", contextMock.Session.GetString("UserRole"));
        }

        #endregion

        #region LecturerController Tests

        [TestMethod]
        public async Task LecturerController_SubmitClaim_GET_ReturnsView()
        {
            var controller = new LecturerController(_context);
            var result = controller.SubmitClaim() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ClaimModel));
        }

        [TestMethod]
        public async Task LecturerController_SubmitClaim_POST_ValidClaim_Saves()
        {
            var controller = new LecturerController(_context);
            var claim = new ClaimModel
            {
                Title = "Test Claim",
                HoursWorked = 10,
                HourlyRate = 100,
                AdditionalNotes = "Test Note"
            };

            var result = await controller.SubmitClaim(claim, null) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Overview", result.ActionName);
            var savedClaim = _context.ClaimModel.FirstOrDefault();
            Assert.IsNotNull(savedClaim);
            Assert.AreEqual("Pending", savedClaim.ClaimStatus);
        }

        [TestMethod]
        public async Task LecturerController_SubmitClaim_POST_InvalidFile_ReturnsViewWithError()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("image/png");

            var controller = new LecturerController(_context);
            var claim = new ClaimModel { Title = "File Test" };

            var result = await controller.SubmitClaim(claim, fileMock.Object) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        #endregion

        #region PCAMController Tests

        [TestMethod]
        public void PCAMController_ClaimList_ReturnsViewWithClaims()
        {
            // Seed a claim
            _context.ClaimModel.Add(new ClaimModel { ClaimId = 1, Title = "Claim 1", HoursWorked = 5, HourlyRate = 100, ClaimStatus = "Pending" });
            _context.SaveChanges();

            var controller = new PCAMController(_context);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var result = controller.ClaimList() as ViewResult;
            Assert.IsNotNull(result);
            var model = result.Model as List<ClaimModel>;
            Assert.AreEqual(1, model.Count);
        }

        [TestMethod]
        public void PCAMController_VerifyClaim_ChangesStatusToVerified()
        {
            _context.ClaimModel.Add(new ClaimModel { ClaimId = 1, Title = "Claim 1", ClaimStatus = "Pending" });
            _context.SaveChanges();

            var controller = new PCAMController(_context);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var result = controller.VerifyClaim(1) as RedirectToActionResult;

            var claim = _context.ClaimModel.First();
            Assert.AreEqual("Verified", claim.ClaimStatus);
            Assert.AreEqual("ClaimList", result.ActionName);
        }

        [TestMethod]
        public void PCAMController_RejectClaim_ChangesStatusToRejected()
        {
            _context.ClaimModel.Add(new ClaimModel { ClaimId = 1, Title = "Claim 1", ClaimStatus = "Pending" });
            _context.SaveChanges();

            var controller = new PCAMController(_context);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var result = controller.RejectClaim(1) as RedirectToActionResult;

            var claim = _context.ClaimModel.First();
            Assert.AreEqual("Rejected", claim.ClaimStatus);
            Assert.AreEqual("ClaimList", result.ActionName);
        }

        #endregion
    }
}
