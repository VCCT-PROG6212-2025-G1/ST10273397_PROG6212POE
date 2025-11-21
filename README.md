# PROG6212POE - Lecturer Claim Management System

## GitHub Link

https://github.com/VCCT-PROG6212-2025-G1/ST10273397_PROG6212POE/

## YouTube Link

https://youtu.be/8ewq0JXQozg

## Overview

This project is a **web-based claim management system** built using **ASP.NET Core MVC** and **C#**. It allows lecturers to submit claims for hours worked, while Programme Coordinators, Academic Managers, and HR personnel can review, verify, approve, or reject these claims. The system supports PDF uploads, role-based workflows, status tracking, user management, and comprehensive reporting capabilities.

---

## Lecturer Feedback

Application now works properly and is bug and error free. And if something still does happen, a video is now provided.

---

## Features

### Lecturer
- Submit new claims with details including:
  - Claim Title
  - Hours Worked
  - Hourly Rate
  - Additional Notes
  - Optional PDF supplementary document
- Auto-calculation of total claim amount
- View submitted claims and their status
- Track claim submission dates

### Programme Coordinator (PC)
- View all submitted claims
- Verify or reject claims with a single click
- Download supplementary documents
- Feedback messages for actions
- Filter and review pending claims

### Academic Manager (AM)
- View all submitted claims
- Approve verified claims or reject them
- Download supplementary documents
- Feedback messages for actions
- Final approval authority for verified claims

### HR (Human Resources)
- **User Management Dashboard**
  - View all registered users in the system
  - Access detailed user information
  - Create new users with role assignment
  - Edit existing user details
  - View user-specific claim history
- **Comprehensive User Details**
  - Personal information display
  - Complete claims history per user
  - Claims statistics (Total, Approved, Pending, Rejected)
  - Total claims amount calculation
  - Download access to claim documents
- **PDF Report Generation**
  - Generate detailed user reports including:
    - User information (name, email, role, hourly rate)
    - Complete claims history
    - Claims summary with status breakdown
    - Total approved claims amount
  - Automated PDF creation using IronPDF
  - Download reports with formatted date stamps
- **User Creation & Management**
  - Create new users with all required fields:
    - First Name and Last Name
    - Email and Password
    - Role assignment (Lecturer, Programme Coordinator, Academic Manager, HR)
    - Hourly rate configuration
  - Edit user information
  - Update user roles and rates
  - Password management

### Login & Role Management
- Role-based login system:
  - Lecturer
  - Programme Coordinator
  - Academic Manager
  - HR (Human Resources)
- Session-based role tracking for secure access to relevant pages
- Access denied handling for unauthorized access attempts

---

## Technologies Used
- **ASP.NET Core MVC (.NET 8.0)**
- **C#**
- **Entity Framework Core 8.0.22**
- **SQL Server (LocalDB)**
- **IronPDF 2025.11.12** (PDF generation)
- **Bootstrap 5** (UI framework)
- **Bootstrap Icons**
- **MSTest & Moq** (Unit testing)

---

## Database Configuration

### Connection String
The application uses SQL Server LocalDB with the following connection string:
```
Server=(localdb)\\mssqllocaldb;Database=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true
```

### Seed Data
The system includes a default HR user:
- **Email**: hr@company.com
- **Password**: HR123
- **Role**: HR
- **User ID**: 1

---

## Project Structure

### Controllers
- **LoginController**: Handles login, logout, and session management
- **LecturerController**: Handles claim submission and lecturer overview
- **PCAMController**: Handles claim review, verification, approval, and rejection for PC and AM roles
- **HRController**: Handles comprehensive HR functions including:
  - User management dashboard
  - User creation and editing
  - User details and claims history viewing
  - PDF report generation

### Models
- **UserModel**: Represents system users with roles and personal information
- **ClaimModel**: Represents claims submitted by lecturers
- **ErrorViewModel**: Standard error handling model

### Views

#### Lecturer
- `SubmitClaim.cshtml`: Form for claim submission with validation
- `Overview.cshtml`: Lecturer claim overview with status badges

#### PCAM
- `PCClaimList.cshtml`: Programme Coordinator claim review dashboard
- `AMClaimList.cshtml`: Academic Manager claim approval dashboard

#### HR
- `Index.cshtml`: User management dashboard with user listing
- `CreateUser.cshtml`: User creation form with role selection
- `Edit.cshtml`: User editing interface with validation
- `Details.cshtml`: Comprehensive user details with claims history and statistics
- `ReportTemplate.cshtml`: PDF report template for user reports

#### Shared
- `_Layout.cshtml`: Main layout with navigation and footer
- `_ValidationScriptsPartial.cshtml`: Client-side validation scripts

#### Login
- `Login.cshtml`: Login form with Bootstrap styling
- `AccessDenied.cshtml`: Access denied page

## File Upload System

### Upload Configuration
- **Allowed Format**: PDF only
- **Maximum Size**: 10 MB
- **Storage Location**: `wwwroot/uploads/`
- **Naming Convention**: GUID-based unique filenames
- **Security**: Content type and size validation

### File Handling
- Server-side validation for file type and size
- Automatic directory creation if not exists
- Original filename preserved for display
- Unique server filename for storage
- Download functionality for authorized users

---

## PDF Report Generation

### IronPDF Integration
The system uses IronPDF's ChromePdfRenderer for professional report generation:

### Report Features
- **User Information Section**: Complete user profile details
- **Claims History Table**: Detailed listing of all user claims
- **Financial Summary**: Total approved claims calculation
- **Professional Styling**: Clean, print-ready formatting
- **Automated Naming**: Reports named with user details and date

### Report Access
- HR-only functionality
- Accessible from user details page
- Generated on-demand
- Direct download to browser

---

## Installation & Setup

### Prerequisites
- Visual Studio 2022 or later
- .NET 8.0 SDK
- SQL Server LocalDB

### Setup Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/VCCT-PROG6212-2025-G1/ST10273397_PROG6212POE/
   ```

2. Open the solution in Visual Studio:
   ```
   PROG6212POE/PROG6212POE.sln
   ```

3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

4. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

5. Ensure the uploads directory exists:
   ```
   PROG6212POE/wwwroot/uploads/
   ```

6. Run the application:
   ```bash
   dotnet run
   ```

7. Access the application at:
   ```
   https://localhost:7090
   ```

---

## Usage

### Login Credentials

#### HR User (Pre-configured)
- **Email**: hr@company.com
- **Password**: HR123
- **Access**: User management, report generation

#### Creating Additional Users
1. Log in as HR user
2. Navigate to "Create New User"
3. Fill in required details:
   - First and Last Name
   - Email address
   - Password
   - Role selection
   - Hourly rate (for lecturers)
4. Submit to create user

### Lecturer Workflow
1. Log in with lecturer credentials
2. Navigate to "Submit Claim"
3. Fill in claim details:
   - Claim title
   - Hours worked
   - Optional notes
   - Upload PDF document (optional)
4. Submit claim
5. View claim status in "Overview"

### Programme Coordinator Workflow
1. Log in with PC credentials
2. Review pending claims in dashboard
3. Click "Verify" to approve claims for AM review
4. Click "Reject" to deny claims with feedback
5. Download supplementary documents for verification

### Academic Manager Workflow
1. Log in with AM credentials
2. Review verified claims in dashboard
3. Click "Approve" for final approval
4. Click "Reject" to deny claims
5. Download and review documents

### HR Workflow
1. Log in with HR credentials
2. **View Users**: See all system users in dashboard
3. **Create Users**: Add new users with role assignment
4. **Edit Users**: Modify user information and rates
5. **View Details**: Access comprehensive user information
6. **Generate Reports**: Create PDF reports for users
7. **Manage Claims**: View all claims across all users

---

## Validation & Business Rules

### Claim Submission
- **Hours Worked**: Maximum 180 hours per month
- **File Upload**: PDF only, maximum 10 MB
- **Required Fields**: Title, Hours Worked
- **Auto-calculation**: Total amount = Hours × Hourly Rate

### User Management
- **Email**: Must be unique and valid format
- **Password**: Required for new users
- **Role**: Must be one of four defined roles
- **Hourly Rate**: Required for lecturers

### Access Control
- Role-based authorization for all features
- Session management for user tracking
- Access denied handling for unauthorized attempts
- Secure file access for claim documents

---

## Error Handling

### Implementation
- Try-catch blocks in all controller actions
- Custom error pages
- User-friendly error messages
- Graceful degradation for missing data
- Logging for debugging (console output)

### Error Scenarios Handled
- Database connection failures
- File upload errors
- Invalid user access attempts
- Missing or invalid data
- PDF generation failures
- Session timeout handling

---

## Testing

### Test Project
Location: `PROG6212POE Testing/PROG6212POE Testing.csproj`

### Test Coverage
- **Login Controller**: Authentication and authorization
- **Lecturer Controller**: Claim submission and validation
- **PCAM Controller**: Claim review and approval workflows
- **HR Controller**: User management operations

### Testing Technologies
- **MSTest**: Testing framework
- **Moq**: Mocking framework for dependencies
- **InMemory Database**: Isolated testing environment

### Running Tests
```bash
dotnet test
```

## AI Declaration

ChatGPT was used to:
- **Add comments** to existing code for clarity

Reference: [AI ChatGPT Conversation](https://chatgpt.com/share/68f94165-1cbc-8012-8319-a47aee0e4e07)

---

## License

This project is created for educational purposes as part of PROG6212 coursework.

---

## Version History

### Version 2.0 (Current)
- Added comprehensive HR module
- Implemented PDF report generation
- Enhanced user management capabilities
- Added claim history tracking
- Improved error handling
- Enhanced UI/UX with Bootstrap 5

### Version 1.0
- Basic claim submission
- Programme Coordinator review
- Academic Manager approval
- Role-based login system
- File upload functionality
