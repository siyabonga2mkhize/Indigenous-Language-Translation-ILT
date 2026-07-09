## Reading Time : 6 Min ; - ) 
# Indigenous Language Phrasebook & Translation Portal  
## SDLC Phase-by-Phase Structure for Your GitHub Project

## Phase 1 – Project Initiation & Requirements Analysis
**Goal:** Fully understand the problem, stakeholders, and success criteria.

**Tasks:**  
- Read the project brief (pages 15–16) at least twice.  
- Identify all **user roles** (Student, Administrator) and their **user stories** (listed in the handbook).  
- Extract **functional requirements** (search, submit translation, favourites, admin approval, statistics).  
- Identify **non‑functional requirements** (performance, responsiveness, security, multi‑language support).  
- Create a **Use Case Diagram** or a simple list of use cases.  
- Define the **scope** – what’s in and out for version 1.0.  
- Write a **Project Vision Statement** (one paragraph).

**Deliverables:**  
- A `docs/` folder containing:
  - `requirements.md` – all functional and non‑functional requirements.
  - `use-cases.md` – list of use cases per role.
  - `vision.md` – your project vision.

**GitHub:**  
- Create a Milestone called **Phase 1 – Initiation**.  
- Create issues for each task and assign them to yourself.  
- Start a **Project Board** with columns: *Backlog, To Do, In Progress, Review, Done*.

---

## Phase 2 – System Design (High‑Level & Detailed)
**Goal:** Design the architecture, database schema, and user interface flows.

**Tasks:**  
- **Database Design** (critical for this project):
  - Identify entities: `User`, `Phrase`, `Translation`, `Category`, `Favourite`, `Submission`, `LanguageStatistic`.
  - Define relationships (e.g., Phrase 1‑to‑many Translation, User 1‑to‑many Submission).
  - Create an **ER Diagram** (use tools like draw.io or DBDiagram).
  - Write SQL scripts to create tables with proper foreign keys, indexes, and seed data.
- **Architecture Design**:  
  - MVC layers: Controllers, Services (business logic), Repository/Data Access.
  - Plan for **ASP.NET Identity** for authentication (Student & Admin roles).
- **UI/UX Wireframes**:  
  - Sketch (or use Figma) main screens: Home/Search, Phrase Detail, Submit Translation, Favourites, Admin Dashboard.
- **API Design** (if any): but mostly MVC actions.
- **Security Plan**: Role‑based authorization, input validation, XSS/CSRF protection.
- **Offline/Performance considerations**: caching? (not required but can be nice).

**Deliverables:**  
- `docs/database-schema.sql` – complete schema with constraints.  
- `docs/erd.png` – visual diagram.  
- `docs/architecture.md` – describe layers and key classes.  
- `docs/wireframes/` – screenshots or links to your wireframes.  

**GitHub:**  
- Milestone **Phase 2 – Design**.  
- Issues: *Create ER diagram*, *Design UI wireframes*, *Define service interfaces*, etc.

---

## Phase 3 – Environment Setup & Project Scaffolding
**Goal:** Set up your development environment, create the solution, and configure the basics.

**Tasks:**  
- Install .NET 6/8 SDK, Visual Studio/VS Code, SQL Server (local or Azure).  
- Create a new ASP.NET MVC project with Individual User Accounts (ASP.NET Identity).  
- Add required NuGet packages: Entity Framework Core, SQL Server provider, maybe Bootstrap 5.  
- Set up **Git repository** (public) and initial commit with `.gitignore`, `README.md` (placeholder).  
- Configure `appsettings.json` – connection string, other settings.  
- Scaffold initial database using EF Core migrations.  
- Seed roles (`Student`, `Administrator`) and a default admin user.  
- Create folder structure (Controllers, Views, Models, Services, Data, Helpers).

**Deliverables:**  
- A working solution that builds and runs with a home page.  
- Database created (can be local).  
- GitHub repository with clean initial structure.

**GitHub:**  
- Milestone **Phase 3 – Setup**.  
- Issues: *Initialize project*, *Setup EF Core*, *Seed roles*, *Configure Git*, etc.

---

## Phase 4 – Core Feature Development (Iteration 1)
**Goal:** Build the essential student-facing features first.

**Tasks (split into manageable issues):**  
- **Authentication & Authorization** – ensure roles work (`[Authorize(Roles="Student,Administrator")]`).  
- **Phrase Management** – create `Phrase` and `Category` models; CRUD for administrators (add/edit/delete categories and phrases).  
- **Search & Filter** – implement a search bar that searches across English text and translations. Use `LIKE` or full‑text search; display results with highlighting.  
- **Phrase Detail View** – show all translations for a phrase, with ability to submit a new translation (if logged in).  
- **Submit Translation** – create a form that posts a new `Translation` with status `Pending`.  
- **Favourites** – allow logged-in students to add/remove phrases to their favourites (junction table).  
- **Profile Page** – show user’s submitted translations and their status, and list of favourites.

**Testing (ongoing):**  
- Write unit tests for service methods (e.g., search logic, favourite toggle).  
- Manual testing of each flow.

**Deliverables:**  
- All above features working locally.  
- Database populated with sample campus phrases in multiple languages.

**GitHub:**  
- Milestone **Phase 4 – Core Features**.  
- Create issues for each feature (e.g., “Implement phrase search”, “Add translation submission form”).  
- Use branches for each feature and merge via pull requests.

---

## Phase 5 – Admin Features & Reporting
**Goal:** Complete the administration panel and statistics.

**Tasks:**  
- **Admin Approval Workflow**:  
  - List all pending submissions with approve/reject buttons.  
  - When approved, set status to `Approved` and make it visible in search; when rejected, store reason and notify user (maybe via view).  
- **Add/Edit Categories and Phrases** – admin‑only CRUD.  
- **Language Usage Statistics**:  
  - Log every search (language selected, category) and phrase view (language).  
  - Display a report view (only for admin) showing most searched languages, categories, etc.  
- **Admin Dashboard** – summary of pending submissions, total phrases, etc.  
- **Role‑based View Protection** – ensure all admin actions are secured with `[Authorize(Roles="Administrator")]`.

**Deliverables:**  
- Full admin panel accessible only to admins.  
- Statistics page.

**GitHub:**  
- Milestone **Phase 5 – Admin & Reports**.  
- Issues: *Create admin approval page*, *Implement stats logging*, *Build admin dashboard*.

---

## Phase 6 – Testing, Bug Fixing & Polish
**Goal:** Ensure the application is robust, user‑friendly, and meets all requirements.

**Tasks:**  
- **Comprehensive Testing**:  
  - Unit tests for critical business logic (e.g., approval workflow, search, points – not applicable here but for other projects).  
  - Integration tests for controllers (optional).  
  - Manual acceptance testing against each user story.  
- **Error Handling**:  
  - Add global exception handling (custom error pages).  
  - Validate all inputs (server‑side and client‑side).  
  - Handle invalid searches, duplicate submissions, etc.  
- **UI/UX Polish**:  
  - Ensure responsive design with Bootstrap 5.  
  - Add loading indicators, success/error messages.  
  - Improve visual consistency.  
- **Performance**:  
  - Add indexing on frequently queried columns (e.g., Phrase.EnglishText).  
  - Consider caching of phrases (optional).  
- **Documentation**:  
  - Write a detailed `README.md` – project description, setup instructions, test credentials, screenshots.  
  - Add inline code comments for complex logic.

**Deliverables:**  
- A thoroughly tested application with no critical bugs.  
- Updated README and code documentation.

**GitHub:**  
- Milestone **Phase 6 – Testing & Polish**.  
- Issues: *Write unit tests for search service*, *Fix mobile layout*, *Add error handling middleware*, etc.

---

## Phase 7 – Deployment & Demo Day Preparation
**Goal:** Make your project live and prepare your pitch.

**Tasks:**  
- **Deploy to Azure App Service** (or other host) with Azure SQL Database.  
- Update connection strings and app settings for production.  
- Run migrations on production database.  
- Test deployed version thoroughly – ensure everything works as locally.  
- **Prepare your 5‑minute pitch** (as per handbook Section 6).  
  - Structure: Problem → Solution → Demo → Tech Stack → Lessons Learned → Future Enhancements.  
- **Polish GitHub Repository**:  
  - Ensure `README.md` has live URL, demo credentials, screenshots.  
  - Clean up commit history (squash if needed).  
  - Add a `CONTRIBUTING.md` or `LICENSE` if desired.  
- **Practice your demo** – record a walkthrough video to time yourself.  
- **Prepare for Q&A** – anticipate technical questions about your design decisions, challenges, and scaling.

**Deliverables:**  
- Live application URL.  
- GitHub repository ready for industry review.  
- A confident presentation.

**GitHub:**  
- Milestone **Phase 7 – Deployment & Demo**.  
- Issues: *Deploy to Azure*, *Write final README*, *Record practice demo*, etc.

---

## How to Use This Structure on GitHub

1. **Create a new repository** (public) with a clear name (e.g., `Phrasebook-Portal`).  
2. **Set up a Project Board** (GitHub Projects) with columns:  
   - **Backlog** – all tasks not yet started.  
   - **Phase 1** … **Phase 7** – you can use milestones or custom fields.  
   - Alternatively, use a simple Kanban board with columns: *To Do*, *In Progress*, *Review*, *Done* – and label issues by phase.  
3. **Create Milestones** for each phase (Phase 1 – Initiation, …, Phase 7 – Deployment).  
4. **Create Issues** for every task listed above. Assign them to yourself, add labels (e.g., `phase-1`, `database`, `ui`, `bug`), and link to the appropriate milestone.  
5. **Use branches** for each feature/fix – open pull requests and merge them after review.  
6. **Update your project board** regularly as you progress – this gives you a clear visual of your status.

---

## Additional Tips from the Handbook

- **Database Design**: Use proper foreign keys, meaningful column names, seed with realistic South African data (isiZulu, Sesotho, etc.).  
- **Security**: Protect all admin actions with `[Authorize]` attributes – never rely on hiding menu items.  
- **Error Handling**: Never show a yellow screen of death – always provide friendly messages.  
- **GitHub**: Write meaningful commit messages (not “fix”), include a professional README.  
- **Demo Day**: Your live URL, GitHub link, and demo credentials must be ready.

---

## Suggested Timeline (Based on the 5‑day Intensive + 4‑week Build)

| Phase | Duration | Target Completion |
|-------|----------|-------------------|
| Phase 1 – Initiation | 1 day | End of Day 1 |
| Phase 2 – Design | 2 days | Day 3 |
| Phase 3 – Setup | 1 day | Day 4 |
| Phase 4 – Core Features | 2 weeks | Week 2 |
| Phase 5 – Admin & Reports | 1 week | Week 3 |
| Phase 6 – Testing & Polish | 1 week | Week 4 |
| Phase 7 – Deployment & Demo Prep | 2 days | Before Demo Day |