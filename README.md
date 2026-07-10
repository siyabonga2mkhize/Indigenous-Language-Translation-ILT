# Indigenous-Language-Translation-ILT-
Here's a comprehensive README.md for your repository. It's structured to give contributors a clear understanding of your project, documentation, workflow, and how they can get involved—all in about a 10-minute read.

---

```markdown
# 🌍 Indigenous Language Phrasebook & Translation Portal

> **A community-driven web application breaking down language barriers on university campuses — across all 11 official South African languages.**

| **Project Status** | **Version** | **Build Phase** | **Demo Day** |
|-------------------|-------------|-----------------|--------------|
| 🚧 In Development | v1.0 | 4-Week Sprint | [Date TBD] |

---

## 📖 10-Minute Read

This README is designed to give you everything you need to understand, contribute to, and navigate this project in about **10 minutes**.

- **Minutes 1-2**: What this project is and why it matters
- **Minutes 3-5**: How the project is organised (docs, code, project board)
- **Minutes 6-8**: How we work (SDLC phases, milestones, issues)
- **Minutes 9-10**: How you can contribute

---

## 🎯 What This Project Does

**The Problem:** At universities like DUT, critical campus information — registration deadlines, accommodation rules, health services — is almost always published only in English. Students comfortable in isiZulu, Sesotho, Xhosa, or any of the other nine official South African languages are left at a disadvantage.

**Our Solution:** A searchable, community-driven phrasebook where students can:

- 🔍 **Search** for campus phrases by keyword or category
- 🌐 **Find translations** across all 11 official South African languages
- ✍️ **Submit translations** in their own language
- ⭐ **Save favourites** for quick access
- ✅ **Administrators** review and approve submissions to ensure quality

**Why It Matters:** Every student deserves equal access to information. This platform transforms the campus experience from one of exclusion to one of empowerment.

---

## 📂 Project Organisation

```
📁 Indigenous-Language-Phrasebook/
│
├── 📁 docs/                          # All project documentation
│   ├── 📄 srs.md                     # Software Requirements Specification (v1.0)
│   ├── 📄 structure.md               # Project workflow & GitHub Project Board guide
│   ├── 📄 requirements.md            # Functional & non-functional requirements
│   ├── 📄 use-cases.md               # Detailed use cases for all user roles
│   ├── 📄 vision.md                  # Project vision statement
│   ├── 📄 scope.md                   # v1.0 scope (IN/OUT)
│   ├── 📄 user-stories.md            # User stories from project brief
│   ├── 📁 diagrams/                  # Visual documentation
│   │   ├── 📄 Requirements-Diagram.png
│   │   ├── 📄 ER-Diagram.png
│   │   └── 📁 use-case-diagrams/     # UML use case diagrams per role
│   └── 📁 tests/                     # Test plans & verification artifacts
│
├── 📁 src/                           # Application source code
│   ├── 📁 Controllers/
│   ├── 📁 Views/
│   ├── 📁 Models/
│   ├── 📁 Services/
│   ├── 📁 Data/
│   └── 📁 Helpers/
│
├── 📄 README.md                      # You are here
├── 📄 .gitignore
└── 📄 LICENSE                        # MIT License
```


## 🛠️ Technology Stack

| Layer | Technology |
|-------|------------|
| **Language** | C# (.NET 6 / .NET 8) |
| **Framework** | ASP.NET MVC |
| **ORM** | Entity Framework Core |
| **Database** | Microsoft SQL Server (Azure SQL) |
| **Authentication** | ASP.NET Identity |
| **Front-End** | Bootstrap 5, Razor Views |
| **Version Control** | Git + GitHub (public repository) |
| **Deployment** | Microsoft Azure App Service |

---

## 🔄 How We Work — The SDLC Approach

We follow a structured **7-phase Software Development Life Cycle (SDLC)**, tracked via GitHub **Milestones** and **Issues**.

| Phase | Focus | Status |
|-------|-------|--------|
| **Phase 1** | Project Initiation & Requirements Analysis |
| **Phase 2** | System Design (ERD, Architecture, Wireframes)  |
| **Phase 3** | Environment Setup & Project Scaffolding |
| **Phase 4** | Core Feature Development (Student Side) |
| **Phase 5** | Admin Features & Reporting |
| **Phase 6** | Testing, Bug Fixing & Polish |
| **Phase 7** | Deployment & Demo Day Preparation |

### 📋 GitHub Project Board

We use a **GitHub Project Board** with the following columns:

```
 To Do → In Progress → Review → Done
```

Each task is tracked as a **GitHub Issue**, labelled for easy filtering:

| Label | Purpose |
|-------|---------|
| `enhancement` | New features or improvements |
| `database` | Schema, migrations, indexes, seeding |
| `backend` | C# code: controllers, services, models, logic |
| `frontend` | Razor Views, CSS, Bootstrap, JavaScript |
| `admin` | Admin-only features or role-based access |
| `bug` | Defects, edge cases, validation failures |
| `documentation` | README, comments, diagrams, user stories |
| `testing` | Unit/integration tests, manual QA |
| `deployment` | Azure, hosting, production config |

📖 **Full workflow guide:** [`docs/structure.md`](docs/structure.md)

---

## 🚀 Getting Started

### Prerequisites

- [.NET 6 or .NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extensions
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (local or Azure SQL)
- [Git](https://git-scm.com/)

### Clone & Run

```bash
# Clone the repository
git clone https://github.com/siyabonga2mkhize/Indigenous-Language-Translation-ILT
cd Indigenous-Language-Phrasebook

# Restore dependencies
dotnet restore

# Update connection string in appsettings.json
# Then run migrations
dotnet ef database update

# Run the application
dotnet run
```

### Test Credentials

| Role | Email | Password |
|------|-------|----------|
| Student | student@example.com | Student123! |
| Administrator | admin@example.com | Admin123! |

---

## 🤝 How to Contribute

We welcome contributions! Here's how you can get involved:

### 1. Pick an Issue

Browse our [GitHub Issues](https://github.com/siyabonga2mkhize/Indigenous-Language-Translation-ILT/issues?q=is%3Aissue+state%3Aopen) — look for labels which you see fit and assign yourself

### 2. Follow the Workflow

```bash
# Create a feature branch
git checkout -b feature/your-feature-name

# Make your changes
# Write meaningful commit messages
git commit -m "feat: add phrase search functionality (REQ-FUNC-006)"

# Push and create a Pull Request
git push origin feature/your-feature-name
```

### 3. Pull Request Requirements

- [ ] Link to the relevant **Issue** (e.g., `Closes #25`)
- [ ] Reference the relevant **SRS requirement ID** (e.g., `Implements REQ-FUNC-006`)
- [ ] Include **tests** where applicable
- [ ] Update **documentation** if needed
- [ ] Ensure all **CI checks** pass

### 4. Code Standards

- Follow **C# naming conventions**
- Write **clean, commented code** for complex logic
- Keep **controllers thin** — business logic belongs in **Services**
- Use **async/await** for database operations
- Write **meaningful commit messages** (not "fix" or "update")

---

## 📊 Project Board & Milestones

All work is tracked on our [GitHub Project Board](https://github.com/siyabonga2mkhize/Indigenous-Language-Translation-ILT/projects).


## 📝 Key Documents for Contributors

### Must-Read Before Contributing

1. **[Project Vision](docs/vision.md)** — Understand the "why" behind this project (2 min)
2. **[Project Scope](docs/scope.md)** — Know what's IN and OUT for v1.0 (3 min)
3. **[SRS Document](docs/srs.md)** — Complete requirements specification (15 min for deep dive)
4. **[Use Cases](docs/use-cases.md)** — Understand user interactions for each role (5 min)
5. **[Structure & Workflow](docs/structure.md)** — How we track work on GitHub (5 min)

### Visual Aids

| Diagram | Description | Location |
|---------|-------------|----------|
| **Requirements Diagram** | Visual mapping of functional requirements | [`docs/diagrams/Requirements-Diagram.png`](docs/diagrams/Requirements-Diagram.png) |
| **Use Case Diagrams** | UML diagrams per user role | [`docs/diagrams/use-case-diagrams/`](docs/diagrams/use-case-diagrams/) |
| **ER Diagram** | Database schema visualisation | [`docs/diagrams/ER-Diagram.png`](docs/diagrams/ER-Diagram.png) |


## 🙏 Acknowledgements

- **Geeked-On** — Founding organisation and programme designer
- **Tech Horizons** — Industry & project review partner
- **DUT Department of Information Technology** — Lab partner and faculty support
- **Sphiwe Khuzwayo** — Founder & Managing Director, Tech Horizons

---

> *"Build something that matters. Every line of code you write during this program is a step toward your future in tech."*
> — Sphiwe Khuzwayo, Founder & Managing Director, Tech Horizons

---

**Happy Coding! 🚀**
```
