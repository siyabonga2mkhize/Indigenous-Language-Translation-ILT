# Software Requirements Specification
## For Indigenous Language Phrasebook & Translation Portal

**Version 1.0**  
**Prepared by:** InnoDevs
**Date:** July 2026

---

## Table of Contents

- [1. Introduction](#1-introduction)
  - [1.1 Document Purpose](#11-document-purpose)
  - [1.2 Product Scope](#12-product-scope)
  - [1.3 Definitions, Acronyms, and Abbreviations](#13-definitions-acronyms-and-abbreviations)
  - [1.4 References](#14-references)
  - [1.5 Document Overview](#15-document-overview)
- [2. Product Overview](#2-product-overview)
  - [2.1 Product Perspective](#21-product-perspective)
  - [2.2 Product Functions](#22-product-functions)
  - [2.3 Product Constraints](#23-product-constraints)
  - [2.4 User Characteristics](#24-user-characteristics)
  - [2.5 Assumptions and Dependencies](#25-assumptions-and-dependencies)
  - [2.6 Apportioning of Requirements](#26-apportioning-of-requirements)
- [3. Requirements](#3-requirements)
  - [3.1 External Interfaces](#31-external-interfaces)
  - [3.2 Functional Requirements](#32-functional-requirements)
  - [3.3 Quality of Service](#33-quality-of-service)
  - [3.4 Compliance](#34-compliance)
  - [3.5 Design and Implementation](#35-design-and-implementation)
- [4. Verification](#4-verification)
- [5. Appendixes](#5-appendixes)

---

## Revision History

| Name | Date | Reason For Changes | Version |
|------|------|--------------------|---------|
| [Your Name] | July 2026 | Initial draft | 0.1 |
| [Your Name] | July 2026 | Completed all sections | 1.0 |

---

## 1. Introduction

### 1.1 Document Purpose

This Software Requirements Specification (SRS) document defines the functional and non-functional requirements for the **Indigenous Language Phrasebook & Translation Portal** – a web-based application designed to break down language barriers on university campuses. This document serves as the authoritative reference for the development team, quality assurance testers, project stakeholders, and industry reviewers. It describes **what** the system must do, not **how** it will be implemented, providing a clear baseline for design, development, verification, and acceptance. The primary audiences include the Geeked-On incubation development team, Tech Horizons industry reviewers, DUT faculty, and Demo Day industry judges who will assess the final product.

### 1.2 Product Scope

The **Indigenous Language Phrasebook & Translation Portal** is a web-based ASP.NET MVC application that provides students with a searchable repository of campus-specific phrases across all eleven official South African languages. The system addresses the communication barrier faced by students whose home languages are not English, by offering translations of critical campus information covering Registration, Accommodation, Health Services, Library, and Academic Support. The platform is community-driven, allowing students to submit translation suggestions, which administrators review and approve to ensure accuracy. Version 1.0 includes: user authentication with role-based access (Student and Administrator), phrase search with category filtering, translation submission workflow, favourites system, student profile with submission status tracking, administrator approval panel, phrase and category management, and basic usage statistics reporting. The system is deployed on Microsoft Azure App Service with Azure SQL Database, accessible via any modern web browser on desktop or mobile devices.

### 1.3 Definitions, Acronyms, and Abbreviations

| Term | Definition |
|------|------------|
| API | Application Programming Interface – A set of definitions and protocols for building and integrating application software |
| ASP.NET Identity | A membership system that adds login functionality to ASP.NET applications, supporting role-based authorization |
| Azure | Microsoft's cloud computing platform for hosting applications and databases |
| C# | A modern, object-oriented programming language developed by Microsoft |
| Category | A grouping of phrases by campus topic (e.g., Registration, Accommodation) |
| CRUD | Create, Read, Update, Delete – The four basic operations for persistent storage |
| DUT | Durban University of Technology – The institution where this project is incubated |
| EF Core | Entity Framework Core – An Object-Relational Mapper (ORM) for .NET |
| Favourite | A phrase saved by a student for quick access |
| GitHub | A web-based platform for version control and collaboration using Git |
| MVC | Model-View-Controller – A software architectural pattern for implementing user interfaces |
| ORM | Object-Relational Mapping – A technique for converting data between incompatible systems |
| Pending | A translation submission status indicating it awaits administrator review |
| Phrase | A piece of campus-related text (e.g., "Where is the registration office?") stored in English |
| Razor View | A view template engine for ASP.NET MVC that enables embedding C# code in HTML |
| SRS | Software Requirements Specification – This document |
| SQL | Structured Query Language – A domain-specific language for managing relational databases |
| UI | User Interface – The visual part of a computer application through which a user interacts |
| WCAG | Web Content Accessibility Guidelines – International standards for web accessibility |
| wwwroot | The root folder for static files in ASP.NET applications (CSS, JavaScript, images) |

### 1.4 References

| Reference | Title | Author/Owner | Version | Location/URL |
|-----------|-------|--------------|---------|--------------|
| [1] | Geeked-On Incubation Program Participant Handbook | Geeked-On / Tech Horizons | 2024 | Provided in project documentation |
| [2] | .NET 8 Documentation | Microsoft | 8.0 | https://docs.microsoft.com/en-us/dotnet/ |
| [3] | ASP.NET Core MVC Documentation | Microsoft | 8.0 | https://docs.microsoft.com/en-us/aspnet/core/mvc/ |
| [4] | Entity Framework Core Documentation | Microsoft | 8.0 | https://docs.microsoft.com/en-us/ef/core/ |
| [5] | Bootstrap 5 Documentation | Bootstrap Team | 5.x | https://getbootstrap.com/docs/5.0/ |
| [6] | SRS Template | jam01 | 1.0 | https://github.com/jam01/SRS-Template |
| [7] | IEC 25010:2011 | ISO/IEC | 2011 | Systems and software Quality Requirements and Evaluation |

**Note:** References [1] is normative (binding) for project scope and requirements. References [2]–[5] are informative (guidance) for implementation. Reference [6] provides the document template structure. Reference [7] defines quality attributes.

### 1.5 Document Overview

This SRS is organised into five main sections. **Section 1 – Introduction** provides an overview of the document, its purpose, scope, definitions, references, and structure. **Section 2 – Product Overview** describes the product's context, high-level functions, constraints, user characteristics, assumptions, and how requirements are apportioned across development phases. **Section 3 – Requirements** is the core of the document, specifying detailed functional requirements, external interfaces, quality of service requirements (performance, security, reliability, availability, observability), compliance requirements, and design/implementation constraints. **Section 4 – Verification** describes how each requirement will be validated and tested. **Section 5 – Appendixes** contains supporting material including use cases, user stories, and additional diagrams. This document is version-controlled via GitHub and follows semantic versioning. All revisions are recorded in the Revision History table above.

---

## 2. Product Overview

### 2.1 Product Perspective

The Indigenous Language Phrasebook & Translation Portal is a **standalone web application** developed as part of the Geeked-On ASP.NET Incubation Program, in partnership with the Durban University of Technology (DUT) Department of Information Technology and Tech Horizons. It is designed to complement the existing campus information ecosystem by providing a centralised, searchable translation repository that currently does not exist at DUT.

**System Context:** The portal operates independently but interfaces with the following external systems:
- **ASP.NET Identity** for authentication and user management (internal to the application).
- **Microsoft SQL Server** (Azure SQL Database) for persistent data storage.
- **Azure App Service** for hosting and deployment.
- **Web Browsers** (Chrome, Firefox, Safari, Edge) for user interaction.

**Ownership:** The system is owned and maintained by the Geeked-On incubation program. Support is provided through the program's online channels (lamgeekedon@gmail.com). No Service Level Agreements (SLAs) are formally defined for Version 1.0, though availability expectations are specified in Section 3.3.4.

### 2.2 Product Functions

The Indigenous Language Phrasebook & Translation Portal provides the following major functional areas:

- **User Management**: Registration and login for Student and Administrator roles using ASP.NET Identity with role-based authorization.
- **Phrase Repository**: Storage and retrieval of campus-specific phrases organised by category (Registration, Accommodation, Health Services, Library, Academic Support), with multi-language translations across all eleven official South African languages.
- **Search & Discovery**: Keyword and category-based searching across English source text and all available translations simultaneously, with matching language highlighting.
- **Community Translation Submission**: Students can submit translation suggestions for any phrase, which are stored with a Pending status for administrator review.
- **Approval Workflow**: Administrators can approve, reject (with reason), or request corrections on pending submissions to ensure only accurate translations are published.
- **Favourites System**: Students can save favourite phrases for quick access and remove them as needed.
- **Student Profile**: Students can view all their submitted translations with current approval status (Pending/Approved/Rejected).
- **Administration Panel**: Administrators can manage phrases (add, edit, deactivate, reactivate), manage categories (add, edit, deactivate), and review pending submissions.
- **Usage Statistics**: Automatic tracking of phrase views and searches (recording language and category) with an admin-only reporting dashboard.
- **Responsive Interface**: Mobile-friendly UI using Bootstrap 5 Razor Views accessible on all device sizes.

### 2.3 Product Constraints

The following constraints shape the design and implementation of the system:

| ID | Constraint Statement | Category | Source |
|----|---------------------|----------|--------|
| CON-001 | The system **shall** be built using C# with ASP.NET MVC framework (.NET 6 or .NET 8) | Technology | Program requirement |
| CON-002 | The system **shall** use Entity Framework Core for database access | Technology | Program requirement |
| CON-003 | The system **shall** use Microsoft SQL Server as the database engine | Technology | Program requirement |
| CON-004 | The system **shall** use ASP.NET Identity for authentication and role-based authorization | Technology | Program requirement |
| CON-005 | The user interface **shall** be developed using Bootstrap 5 Razor Views | Technology | Program requirement |
| CON-006 | The system **shall** be deployed to Microsoft Azure App Service with Azure SQL Database | Deployment | Program requirement |
| CON-007 | Source code **shall** be version-controlled using Git and hosted on a public GitHub repository | Process | Program requirement |
| CON-008 | The system **must** support all eleven official South African languages | Domain | Project brief |
| CON-009 | The system **must** handle user data in accordance with South African data protection principles (POPIA) | Regulatory | Compliance |
| CON-010 | The project **must** be completed within the 4-week build phase following the 5-day intensive training | Schedule | Program timeline |

### 2.4 User Characteristics

The system has two primary user groups with distinct characteristics:

| User Class | Description | Access Level | Frequency | Technical Skill | Goals |
|------------|-------------|--------------|-----------|-----------------|-------|
| **Student** | A registered university student who uses the portal to find translations and contribute suggestions | Low | Daily/Weekly | Basic (web browsing) | Find campus information in their preferred language; contribute to the community; save useful phrases |
| **Administrator** | A privileged user responsible for content management and quality control (typically a programme coordinator, lecturer, or designated staff member) | Full | Weekly | Moderate (web application usage) | Ensure translation accuracy; manage content relevance; analyse usage patterns |

**Accessibility Considerations:** All interface elements must meet WCAG 2.1 Level AA standards for accessibility, including proper contrast ratios, keyboard navigability, and screen reader support. The interface must support users with visual, auditory, and motor impairments.

**Language Considerations:** The user interface itself is presented in English, but the content (phrase repository) supports all eleven official South African languages. Error messages and system feedback are provided in clear, simple English.

### 2.5 Assumptions and Dependencies

**Assumptions:**

| ID | Assumption | Impact if False |
|----|------------|-----------------|
| ASS-001 | Students have access to internet-connected devices (smartphones, tablets, laptops) to access the portal | The portal would not be usable by the target audience |
| ASS-002 | The DUT campus will promote the portal to students to drive adoption | The phrasebook would not achieve critical mass of contributions |
| ASS-003 | Sufficient administrators are available to review pending translations within a reasonable timeframe | The approval workflow would become a bottleneck |
| ASS-004 | The eleven official languages are defined as per the South African Constitution | The system's language list would be incorrect |
| ASS-005 | Internet connectivity is available for the deployed Azure application | Users would be unable to access the portal |
| ASS-006 | DUT campus categories (Registration, Accommodation, etc.) remain stable | The system's content organisation would become outdated |

**Dependencies:**

| ID | Dependency | Owner | Impact if Unavailable |
|----|------------|-------|----------------------|
| DEP-001 | Microsoft Azure App Service availability | Microsoft | Application would be inaccessible |
| DEP-002 | Azure SQL Database availability | Microsoft | Data persistence would fail |
| DEP-003 | GitHub repository hosting | GitHub | Code version control and collaboration would be impacted |
| DEP-004 | ASP.NET and Entity Framework Core NuGet packages | Microsoft/Open Source | Development would be halted |
| DEP-005 | DUT IT Department lab access for 5-day intensive | DUT | Training phase would be disrupted |

### 2.6 Apportioning of Requirements

Requirements are apportioned across development phases as follows:

| Phase | Focus Area | Key Requirements |
|-------|------------|------------------|
| **Phase 1 – Initiation** | Project planning and documentation | All requirements identified and documented |
| **Phase 2 – Design** | System architecture and database design | Database schema, ER diagram, architectural decisions |
| **Phase 3 – Setup** | Development environment and scaffolding | CON-001 through CON-007, initial migrations |
| **Phase 4 – Core Features (Student)** | Search, submissions, favourites, profile | REQ-FUNC-003 through REQ-FUNC-012 |
| **Phase 5 – Admin Features** | Approval workflow, management, reports | REQ-FUNC-013 through REQ-FUNC-021 |
| **Phase 6 – Testing & Polish** | Quality assurance, error handling, responsiveness | REQ-PERF-001, REQ-SEC-001 through REQ-SEC-005, REQ-REL-001 |
| **Phase 7 – Deployment & Demo** | Azure deployment, demo preparation | CON-006, REQ-AVAIL-001 |

**Note:** AI/ML requirements are **not applicable** for Version 1.0 and are excluded from this SRS.

---

## 3. Requirements

### 3.1 External Interfaces

#### 3.1.1 User Interfaces

The system provides a web-based user interface accessed through standard web browsers. The UI shall:

- Follow a consistent layout with a navigation bar, main content area, and footer.
- Use **Bootstrap 5** framework for responsive design that adapts to screen sizes (mobile, tablet, desktop).
- Implement **Razor Views** with C# code-behind for dynamic content rendering.
- Support **WCAG 2.1 Level AA** accessibility standards.
- Display user-friendly error and validation messages using Bootstrap alerts.
- Use a **South African context** appropriate colour scheme and typography.

**Key UI Pages:**

| Page | Description | Access |
|------|-------------|--------|
| Home/Landing | Welcome page with search bar and featured categories | Public |
| Register | New student account creation | Public |
| Login | Existing user authentication | Public |
| Search Results | List of phrases matching search criteria with highlighted matches | Authenticated |
| Phrase Detail | Full phrase view with all translations and "Submit Translation" button | Authenticated |
| Submit Translation | Form for submitting a new translation suggestion | Student |
| My Profile | Student's submitted translations with status and favourites list | Student |
| My Favourites | List of phrases the student has saved | Student |
| Admin Dashboard | Overview of pending submissions, phrases, and categories | Administrator |
| Admin Pending | List of pending translation submissions with Approve/Reject actions | Administrator |
| Admin Phrases | CRUD management for phrases | Administrator |
| Admin Categories | CRUD management for categories | Administrator |
| Admin Statistics | Language usage report with charts/tables | Administrator |

#### 3.1.2 Hardware Interfaces

The system has no direct hardware interfaces. It operates as a web application accessed via standard client hardware (desktop computers, laptops, tablets, smartphones) through web browsers. No specialised hardware is required.

#### 3.1.3 Software Interfaces

The system integrates with the following software components:

| Interface | Component | Type | Communication | Purpose |
|-----------|-----------|------|---------------|---------|
| **ASP.NET Identity** | Authentication system | Internal | Method calls | User registration, login, role management |
| **Entity Framework Core** | Object-Relational Mapper | Internal | LINQ queries | Database operations (CRUD) |
| **SQL Server** | Relational database | External | T-SQL via EF Core | Persistent data storage and retrieval |
| **Azure App Service** | Web hosting platform | External | HTTP/HTTPS | Application hosting and deployment |
| **Azure SQL Database** | Cloud database service | External | T-SQL via EF Core | Production data storage |
| **Web Browsers** | Client interface | External | HTTP/HTTPS (REST) | User interaction and UI rendering |

### 3.2 Functional Requirements

#### 3.2.1 User Management

- **ID:** REQ-FUNC-001
- **Title:** User Registration
- **Statement:** The system **shall** allow new users to create accounts by providing email, password, and confirming password.
- **Rationale:** Enables personalised access to student features.
- **Acceptance Criteria:** 
  - Registration form validates email format and password strength
  - New accounts are assigned the "Student" role by default
  - Duplicate email addresses are rejected with an error message
- **Verification Method:** Test
- **More Information:** Uses ASP.NET Identity defaults; maps to Use Cases UC-STU-01, UC-STU-02

---

- **ID:** REQ-FUNC-002
- **Title:** User Login
- **Statement:** The system **shall** authenticate users using email and password credentials, establishing a session upon successful authentication.
- **Rationale:** Securely identifies users to enforce role-based access.
- **Acceptance Criteria:**
  - Invalid credentials return an error message without exposing sensitive information
  - Successful login redirects to the home page
  - Session persists user identity across requests
- **Verification Method:** Test
- **More Information:** Uses ASP.NET Identity; account lockout after multiple failed attempts (default: 5 attempts)

---

- **ID:** REQ-FUNC-003
- **Title:** Role-Based Authorization
- **Statement:** The system **shall** enforce access control using the `[Authorize]` attribute at the controller and action level, restricting actions to authenticated users with specific roles (Student or Administrator).
- **Rationale:** Prevents unauthorised access to restricted functionality.
- **Acceptance Criteria:**
  - Unauthenticated users cannot access any page requiring authentication
  - Students cannot access administrator-only pages (even by URL manipulation)
  - Administrators can access all pages
- **Verification Method:** Test
- **More Information:** Never relies on hiding UI elements for security; all restrictions enforced server-side.

#### 3.2.2 Phrase Repository

- **ID:** REQ-FUNC-004
- **Title:** Store Phrase Repository
- **Statement:** The system **shall** store phrases with English source text, category association, creation date, and active/inactive status.
- **Rationale:** Provides the foundation of the phrasebook content.
- **Acceptance Criteria:**
  - Each phrase belongs to exactly one category
  - Categories are limited to: Registration, Accommodation, Health Services, Library, Academic Support
  - Deactivated phrases are hidden from search results
- **Verification Method:** Test / Inspection
- **More Information:** Database schema includes Phrase table with Category foreign key.

---

- **ID:** REQ-FUNC-005
- **Title:** Store Multi-Language Translations
- **Statement:** The system **shall** store translations for phrases as separate records, each containing the translation text, target language (one of 11 official South African languages), and status (Pending/Approved/Rejected).
- **Rationale:** Supports the multi-language nature of the phrasebook.
- **Acceptance Criteria:**
  - Supports all 11 official South African languages
  - Each language can have at most one approved translation per phrase
  - Pending translations do not appear in search results
- **Verification Method:** Test / Inspection
- **More Information:** Languages: isiZulu, isiXhosa, Afrikaans, English, Sepedi, Sesotho, Setswana, Siswati, Tshivenda, Xitsonga, isiNdebele.

#### 3.2.3 Search & Discovery

- **ID:** REQ-FUNC-006
- **Title:** Search Phrases by Keyword and Category
- **Statement:** The system **shall** allow authenticated students to search for phrases by entering a keyword and optionally selecting a category, searching across English source text and all approved translations.
- **Rationale:** Enables students to quickly find relevant campus information.
- **Acceptance Criteria:**
  - Search results include phrases where the keyword matches English text or any approved translation
  - Results can be filtered by category
  - Matching language is highlighted in results
  - Empty search returns all phrases (or a "no results" message)
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-STU-03. Search uses SQL `LIKE` or full-text search.

---

- **ID:** REQ-FUNC-007
- **Title:** View Phrase Details
- **Statement:** The system **shall** display a phrase's English text, category, and all approved translations when a student clicks on a search result.
- **Rationale:** Provides complete information about a phrase.
- **Acceptance Criteria:**
  - Shows English text prominently
  - Lists all approved translations by language
  - Displays a "Submit Translation" button for students
  - Invalid phrase ID shows "Phrase not found" error
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-STU-04.

#### 3.2.4 Translation Submission

- **ID:** REQ-FUNC-008
- **Title:** Submit Translation Suggestion
- **Statement:** The system **shall** allow authenticated students to suggest a translation for a specific phrase in a specific language, saving it with a "Pending" status.
- **Rationale:** Enables community-driven content growth.
- **Acceptance Criteria:**
  - Form validates target language selection and non-empty translation text
  - Prevents duplicate submissions for the same phrase and language (with warning)
  - Submissions are stored with "Pending" status
  - Confirmation message displayed to the student
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-STU-05.

---

- **ID:** REQ-FUNC-009
- **Title:** View Submitted Translations Status
- **Statement:** The system **shall** display on the student's profile page all translations they have submitted, along with their current status (Pending/Approved/Rejected) and any rejection reason.
- **Rationale:** Provides transparency and feedback to contributors.
- **Acceptance Criteria:**
  - Lists all submissions by the logged-in student
  - Shows status badge (Pending/Approved/Rejected)
  - Displays rejection reason if rejected
  - "No submissions" message for students without contributions
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-STU-06.

#### 3.2.5 Favourites System

- **ID:** REQ-FUNC-010
- **Title:** Save Phrase to Favourites
- **Statement:** The system **shall** allow authenticated students to save any active phrase to their personal favourites list, stored in a junction table.
- **Rationale:** Enables quick access to frequently used phrases.
- **Acceptance Criteria:**
  - Button toggles between "Add to Favourites" and "Remove from Favourites"
  - Prevents duplicate favourites
  - Works for phrases viewed in search results and detail pages
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-STU-07.

---

- **ID:** REQ-FUNC-011
- **Title:** Remove Phrase from Favourites
- **Statement:** The system **shall** allow authenticated students to remove a phrase from their favourites list.
- **Rationale:** Allows students to curate their favourites.
- **Acceptance Criteria:**
  - Remove button functions from favourites list and phrase detail views
  - Removed phrase no longer appears in favourites list
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-STU-08.

---

- **ID:** REQ-FUNC-012
- **Title:** View Favourites List
- **Statement:** The system **shall** display all phrases a student has saved to their favourites, accessible from the navigation menu.
- **Rationale:** Provides a personalised quick-access view.
- **Acceptance Criteria:**
  - Displays favourite phrases in a list or grid format
  - Each phrase links to its detail view
  - "No favourites" message for students without favourites
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-STU-09.

#### 3.2.6 Administration

- **ID:** REQ-FUNC-013
- **Title:** Approve Translation Submission
- **Statement:** The system **shall** allow administrators to approve pending translation submissions, changing their status to "Approved" and making them visible in search results.
- **Rationale:** Ensures only accurate translations are published.
- **Acceptance Criteria:**
  - Admin sees list of all pending submissions
  - Approve button changes status and publishes translation
  - Translation appears in search results immediately
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-ADM-01.

---

- **ID:** REQ-FUNC-014
- **Title:** Reject Translation Submission
- **Statement:** The system **shall** allow administrators to reject pending translation submissions with a reason, changing status to "Rejected" and storing the reason.
- **Rationale:** Provides feedback to submitters and maintains quality.
- **Acceptance Criteria:**
  - Admin must provide a rejection reason
  - Rejected submission shows reason on student's profile
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-ADM-02.

---

- **ID:** REQ-FUNC-015
- **Title:** Request Translation Correction
- **Statement:** The system **shall** allow administrators to request corrections on pending submissions, keeping status as "Pending" and providing instructions to the submitter.
- **Rationale:** Provides a middle ground between approval and rejection.
- **Acceptance Criteria:**
  - Admin provides correction instructions
  - Submission remains "Pending"
  - Instructions visible to student on profile
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-ADM-03.

---

- **ID:** REQ-FUNC-016
- **Title:** Add New Phrase
- **Statement:** The system **shall** allow administrators to add new phrases to the repository with English text and category assignment.
- **Rationale:** Expands the phrasebook content.
- **Acceptance Criteria:**
  - Form validates English text (non-empty) and category selection
  - Prevents duplicate phrases (warning shown)
  - New phrase is immediately active and searchable
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-ADM-04.

---

- **ID:** REQ-FUNC-017
- **Title:** Edit Existing Phrase
- **Statement:** The system **shall** allow administrators to edit the English text and category of existing phrases.
- **Rationale:** Corrects or updates existing content.
- **Acceptance Criteria:**
  - Edit form pre-populated with current values
  - Changes are saved and visible immediately
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-ADM-05.

---

- **ID:** REQ-FUNC-018
- **Title:** Deactivate Phrase (Soft Delete)
- **Statement:** The system **shall** allow administrators to deactivate phrases (soft delete), marking them inactive so they do not appear in search results while retaining data.
- **Rationale:** Removes inappropriate or outdated content without permanent deletion.
- **Acceptance Criteria:**
  - Deactivated phrase hidden from search results
  - Data remains in database with `IsActive = false`
  - Admin can reactivate later
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-ADM-06.

---

- **ID:** REQ-FUNC-019
- **Title:** Manage Categories
- **Statement:** The system **shall** allow administrators to add, edit, and deactivate categories.
- **Rationale:** Keeps the category system flexible and relevant.
- **Acceptance Criteria:**
  - Add category with name and description (name unique)
  - Edit category name/description
  - Deactivate category (soft delete)
  - Deactivated categories hidden from phrase assignment dropdown
- **Verification Method:** Test
- **More Information:** Maps to Use Cases UC-ADM-08 through UC-ADM-10.

#### 3.2.7 Usage Statistics

- **ID:** REQ-FUNC-020
- **Title:** Track Usage Statistics
- **Statement:** The system **shall** automatically log every phrase view and search action, recording the target language and category, into a LanguageStatistic table.
- **Rationale:** Provides data for administrators to prioritise translation efforts.
- **Acceptance Criteria:**
  - Every phrase view (phrase detail page) is logged
  - Every search is logged with selected language (if any) and category (if any)
  - Logs include timestamp and user (if authenticated)
- **Verification Method:** Test / Inspection
- **More Information:** Anonymous tracking for unauthenticated users; authenticated tracking for logged-in users.

---

- **ID:** REQ-FUNC-021
- **Title:** View Usage Statistics Report
- **Statement:** The system **shall** allow administrators to view aggregated usage statistics showing most searched languages, most viewed categories, and popular phrases.
- **Rationale:** Informs content prioritisation decisions.
- **Acceptance Criteria:**
  - Dashboard shows top searched languages
  - Dashboard shows most viewed categories
  - Dashboard shows most viewed phrases
  - "No data" message when statistics are empty
- **Verification Method:** Test
- **More Information:** Maps to Use Case UC-ADM-11.

### 3.3 Quality of Service

#### 3.3.1 Performance

| ID | Requirement | Target | Measurement Method |
|----|-------------|--------|-------------------|
| REQ-PERF-001 | Search results **shall** return within 2 seconds for repositories with up to 1,000 phrases | ≤2 seconds | Load testing (100 concurrent searches) |
| REQ-PERF-002 | Phrase detail page **shall** load within 1 second | ≤1 second | Page load timing (server response + rendering) |
| REQ-PERF-003 | Database queries **shall** be optimised with appropriate indexes on frequently searched columns | N/A | Query execution plan analysis |
| REQ-PERF-004 | The system **shall** support up to 50 concurrent users without significant performance degradation | ≤20% increase in response time | Load testing with 50 concurrent users |

#### 3.3.2 Security

| ID | Requirement | Category |
|----|-------------|----------|
| REQ-SEC-001 | All restricted actions **must** be protected at the controller level using `[Authorize(Roles="...")]` attributes | Authorization |
| REQ-SEC-002 | User passwords **must** be hashed and salted using ASP.NET Identity's built-in password hasher | Confidentiality |
| REQ-SEC-003 | All user inputs **must** be validated server-side to prevent SQL injection and XSS attacks | Integrity |
| REQ-SEC-004 | Connection strings and sensitive settings **must** be stored in `appsettings.json` and never hard-coded | Confidentiality |
| REQ-SEC-005 | Admin-only pages **must** return a 401/403 response if accessed by unauthorised users (not just redirect to login) | Authorization |

**Security Controls Summary:**

- **Authentication:** ASP.NET Identity with configurable password policies (minimum length, complexity requirements).
- **Authorization:** Role-based authorization at controller and action level using `[Authorize]` attributes.
- **Data Protection:** Passwords hashed using ASP.NET Identity's `PasswordHasher<TUser>`.
- **Input Validation:** Server-side validation using Data Annotations and custom validation; client-side validation for UX.
- **CSRF Protection:** Anti-forgery tokens enabled by default in ASP.NET MVC forms.
- **SQL Injection:** Entity Framework Core uses parameterised queries, preventing injection.

#### 3.3.3 Reliability

| ID | Requirement | Acceptance Criteria |
|----|-------------|---------------------|
| REQ-REL-001 | The system **shall** handle all validation failures gracefully without crashing (no yellow screen of death) | Invalid inputs return validation errors; invalid record IDs return 404 with custom error page |
| REQ-REL-002 | The system **shall** implement global exception handling to capture and log unhandled exceptions | Custom error page displayed; errors logged to application logs |
| REQ-REL-003 | Database operations **must** handle connection failures with retry logic (using EF Core retry policies) | Transient faults do not crash the application |

#### 3.3.4 Availability

| ID | Requirement | Target |
|----|-------------|--------|
| REQ-AVAIL-001 | The deployed system **shall** have at least 95% uptime during the Demo Day period and program evaluation window | ≥95% availability |
| REQ-AVAIL-002 | Scheduled maintenance (if any) **shall** be communicated to users in advance | N/A (no formal SLAs for v1.0) |
| REQ-AVAIL-003 | The system **shall** start up and recover from a restart within 30 seconds | ≤30 seconds |

#### 3.3.5 Observability

| ID | Requirement | Details |
|----|-------------|---------|
| REQ-OBS-001 | The system **shall** log all errors and exceptions to application logs | Use Azure Application Insights or ASP.NET logging |
| REQ-OBS-002 | The system **shall** provide structured logging for key user actions (searches, submissions, approvals) | Logs include user ID, timestamp, action type, and outcome |
| REQ-OBS-003 | Search and view statistics **shall** be stored in the database for reporting purposes | LanguageStatistic table stores search/view data |

### 3.4 Compliance

| ID | Requirement | Authority |
|----|-------------|-----------|
| REQ-COMP-001 | The system **must** comply with South African data protection principles as per the Protection of Personal Information Act (POPIA) | POPIA |
| REQ-COMP-002 | The user interface **shall** comply with WCAG 2.1 Level AA accessibility standards | WCAG 2.1 |
| REQ-COMP-003 | The source code **must** include appropriate licensing information (MIT License recommended) | Open source best practices |

### 3.5 Design and Implementation

#### 3.5.1 Installation

The system is deployed to Azure App Service and does not require user-side installation. For development, the following prerequisites are required:

- .NET 6 or .NET 8 SDK
- Visual Studio 2022 or Visual Studio Code with C# extensions
- SQL Server (local or Azure SQL)
- Git

**Deployment Steps:**
1. Clone repository from GitHub
2. Update connection string in `appsettings.json`
3. Run EF Core migrations (`dotnet ef database update`)
4. Deploy to Azure App Service via Visual Studio publish or GitHub Actions

#### 3.5.2 Build and Delivery

- Build system: .NET SDK with `dotnet build` command
- Source control: GitHub public repository
- Branch strategy: Main branch for stable code; feature branches for development
- Deployment: Azure App Service continuous deployment (optional)

#### 3.5.3 Distribution

Single-instance deployment on Azure App Service. No multi-region distribution is required for Version 1.0. Scalability is managed by Azure App Service auto-scaling if configured.

#### 3.5.4 Maintainability

- Coding standards: Follow C# naming conventions and SOLID principles
- Documentation: Inline comments for complex logic; README with setup instructions
- Repository structure: Clear folder organisation (Controllers, Views, Models, Services, Data, Helpers)
- Unit tests: Minimum 70% code coverage for service layer

#### 3.5.5 Reusability

The following components are designed for potential reuse:
- **PhraseService**: Core business logic for phrase and translation management
- **StatisticsService**: Usage tracking and reporting
- **SeedData**: Database seeding for development

#### 3.5.6 Portability

The system is designed for Azure App Service but can run on any platform supporting .NET 6/8 and SQL Server:

- Windows: Visual Studio / IIS
- Linux: .NET runtime with SQL Server
- macOS: Visual Studio / VS Code with SQL Server

**Platform Support:** Azure App Service (primary); any .NET-compatible web host (secondary).

#### 3.5.7 Cost

- Azure App Service: Basic tier (B1) or Free tier (if available) for prototype
- Azure SQL Database: Basic tier (DTU-based) for prototype
- Estimated monthly cost: ~$15–$30 USD for prototype

#### 3.5.8 Deadline

- **5-Day Intensive Training:** [Date] – [Date]
- **4-Week Build Phase:** [Date] – [Date]
- **Demo Day:** [Date]

All requirements must be completed and deployed by the Demo Day date.

#### 3.5.9 Proof of Concept

**Not applicable** – this is a full product build phase, not a proof of concept.

#### 3.5.10 Change Management

- **Breaking Changes:** Require version bump (major version increment)
- **Additive Features:** Require minor version increment
- **Bug Fixes:** Require patch version increment
- **Approval:** Changes must be approved by programme lead
- **Documentation:** All changes must be reflected in updated documentation

---

## 4. Verification

| Requirement ID | Verification Method | Test/Artifact Link | Status | Evidence |
|----------------|---------------------|--------------------|--------|----------|
| REQ-FUNC-001 | Test | `tests/UC-STU-01.md` | Pending | TBD |
| REQ-FUNC-002 | Test | `tests/UC-STU-02.md` | Pending | TBD |
| REQ-FUNC-003 | Test | `tests/auth-role.md` | Pending | TBD |
| REQ-FUNC-004 | Inspection | `docs/database-schema.sql` | Pending | TBD |
| REQ-FUNC-005 | Test | `tests/translations.md` | Pending | TBD |
| REQ-FUNC-006 | Test | `tests/UC-STU-03.md` | Pending | TBD |
| REQ-FUNC-007 | Test | `tests/UC-STU-04.md` | Pending | TBD |
| REQ-FUNC-008 | Test | `tests/UC-STU-05.md` | Pending | TBD |
| REQ-FUNC-009 | Test | `tests/UC-STU-06.md` | Pending | TBD |
| REQ-FUNC-010 | Test | `tests/UC-STU-07.md` | Pending | TBD |
| REQ-FUNC-011 | Test | `tests/UC-STU-08.md` | Pending | TBD |
| REQ-FUNC-012 | Test | `tests/UC-STU-09.md` | Pending | TBD |
| REQ-FUNC-013 | Test | `tests/UC-ADM-01.md` | Pending | TBD |
| REQ-FUNC-014 | Test | `tests/UC-ADM-02.md` | Pending | TBD |
| REQ-FUNC-015 | Test | `tests/UC-ADM-03.md` | Pending | TBD |
| REQ-FUNC-016 | Test | `tests/UC-ADM-04.md` | Pending | TBD |
| REQ-FUNC-017 | Test | `tests/UC-ADM-05.md` | Pending | TBD |
| REQ-FUNC-018 | Test | `tests/UC-ADM-06.md` | Pending | TBD |
| REQ-FUNC-019 | Test | `tests/UC-ADM-08-10.md` | Pending | TBD |
| REQ-FUNC-020 | Test | `tests/statistics.md` | Pending | TBD |
| REQ-FUNC-021 | Test | `tests/UC-ADM-11.md` | Pending | TBD |
| REQ-PERF-001 | Test | `tests/performance.md` | Pending | TBD |
| REQ-PERF-002 | Test | `tests/performance.md` | Pending | TBD |
| REQ-PERF-003 | Analysis | `docs/indexes.md` | Pending | TBD |
| REQ-SEC-001 | Inspection | Code review | Pending | TBD |
| REQ-SEC-002 | Analysis | Code review | Pending | TBD |
| REQ-SEC-003 | Test | Security scanning | Pending | TBD |
| REQ-SEC-004 | Inspection | Code review | Pending | TBD |
| REQ-SEC-005 | Test | Manual test | Pending | TBD |
| REQ-REL-001 | Test | Manual error testing | Pending | TBD |
| REQ-REL-002 | Test | Manual error testing | Pending | TBD |
| REQ-AVAIL-001 | Analysis | Monitoring | Pending | TBD |
| REQ-COMP-001 | Inspection | Privacy assessment | Pending | TBD |
| REQ-COMP-002 | Analysis | Accessibility audit | Pending | TBD |
| REQ-COMP-003 | Inspection | License file | Pending | TBD |

---

## 5. Appendixes

### Appendix A: Use Cases

*[Refer to the detailed use cases documented separately in `docs/use-cases.md`]*

### Appendix B: User Stories

*[Refer to user stories documented in `docs/user-stories.md`]*

### Appendix C: Project Scope

*[Refer to project scope documented in `docs/scope.md`]*

### Appendix D: Vision Statement

*[Refer to vision statement documented in `docs/vision.md`]*

### Appendix E: ER Diagram

*[To be created in Phase 2 – Design]*

---

**End of SRS Document**

---
