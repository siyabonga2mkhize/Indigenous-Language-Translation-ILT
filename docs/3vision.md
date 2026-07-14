## Reading Time : 4 Min ;-) 
# 🎯 Project Scope – Version 1.0

## What's IN Scope (Version 1.0)

### Core Features
- **User Authentication**: Registration and login using ASP.NET Identity with two roles: `Student` and `Administrator`.
- **Phrase Management**: Store and display phrases with English text, category association, and multi-language translations (all 11 official South African languages).
- **Category System**: Predefined campus categories: *Registration, Accommodation, Health Services, Library, Academic Support*.
- **Search Functionality**: Search across English text and all available translations simultaneously with keyword matching and category filtering.
- **Translation Submission**: Logged-in students can submit translation suggestions for any phrase in their preferred language.
- **Approval Workflow**: Administrators can approve, reject (with reason), or request corrections on pending submissions.
- **Favourites System**: Students can save/remove phrases to a personal favourites list and view them quickly.
- **Student Profile**: Display all translations submitted by the student with their current approval status (Pending/Approved/Rejected).
- **Usage Statistics**: Automatic tracking of phrase views and searches (language and category).
- **Admin Dashboard**: Pending submissions list, phrase/category management, and usage reports.
- **Responsive UI**: Bootstrap 5 Razor Views with mobile-friendly layout.

### Technical Requirements
- **Backend**: C# with ASP.NET MVC (.NET 6/8)
- **Database**: Microsoft SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Identity with role-based authorization
- **Deployment**: Azure App Service + Azure SQL Database
- **Version Control**: Public GitHub repository with professional README

### Documentation
- SRS Document (Software Requirements Specification)
- README with setup instructions, screenshots, and test credentials
- ER Diagram and database schema
- Inline code comments for complex logic

---

## What's OUT of Scope (Version 1.0 – Future Enhancements)

### Features We're NOT Building Now
- **Email Notifications**: Students are not notified via email when their translation is approved/rejected (will be displayed on their profile instead).
- **Social Features**: No commenting, rating, or upvoting system for translations.
- **Mobile App**: No native mobile application – web-based only (responsive for mobile browsers).
- **Offline Support**: No offline caching or PWA capabilities.
- **Voice/Audio Translations**: No audio pronunciation features.
- **Advanced Analytics**: No advanced BI dashboards or exportable reports (just basic usage statistics).
- **Third-party Translations**: No integration with external translation APIs (e.g., Google Translate) – community-driven only.
- **Multiple Administrators**: No granular admin permissions (all admins have full access).
- **Translation History**: No version tracking or edit history for translations.
- **Bulk Import/Export**: No CSV/Excel import/export for phrases or translations.
- **Multi-tenant Support**: Single campus (DUT) only – not configurable for other institutions.
- **Real-time Notifications**: No WebSocket/SignalR real-time updates.

### Technical Exclusions
- No microservices architecture (monolithic MVC application).
- No containerization (Docker/Kubernetes) – deployed directly to Azure.
- No CI/CD pipeline beyond basic deployment.
- No load balancing or horizontal scaling.
- No advanced caching (beyond standard session state).

---

## Scope Boundaries Summary

| Category | In Scope ✅ | Out of Scope ❌ |
| :--- | :--- | :--- |
| **User Management** | Registration, Login, Roles (Student/Admin) | Email verification, Password reset, Social login |
| **Phrase Content** | English text, Category, Multi-language translations | Audio pronunciations, Image attachments, Videos |
| **Search** | Keyword + Category search with highlighting | Advanced filters, Fuzzy search, Auto-suggest |
| **Submissions** | Student suggestions with Pending/Approved/Rejected | Voting on translations, Collaborative editing |
| **Favourites** | Save/Remove personal favourites | Sharing favourites, Favourite categories |
| **Reporting** | Basic usage statistics (languages/categories) | Advanced analytics, Exportable reports |
| **Notifications** | Status visible on user profile | Email/SMS notifications |
| **Deployment** | Azure App Service + Azure SQL | Docker, Kubernetes, Multi-region |

---

# 🚀 Project Vision Statement

The **Indigenous Language Phrasebook & Translation Portal** is a community-driven web application designed to break down language barriers on university campuses by providing students with a searchable repository of campus-specific phrases across all eleven official South African languages. Built on the ASP.NET MVC framework, this platform empowers students to find essential information—from registration procedures to health services—in their mother tongue, fostering inclusivity and equal access to education. Students can actively contribute by submitting translation suggestions, which are then reviewed and approved by administrators to ensure accuracy and relevance. By combining community contribution with administrative quality control, the portal creates a living, evolving resource that adapts to students' needs while celebrating South Africa's rich linguistic diversity. The system also provides valuable usage analytics to administrators, enabling data-driven decisions on which translations to prioritise next. In doing so, the project transforms the campus experience from one of exclusion to one of empowerment, where every student—regardless of their home language—can navigate university life with confidence and ease.
