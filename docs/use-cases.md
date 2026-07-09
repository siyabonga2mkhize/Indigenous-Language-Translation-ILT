# 📋 Use Cases for Indigenous Language Phrasebook & Translation Portal

---

## 👤 ACTOR: Student

**Description:** A registered user who can search the phrasebook, view translations, submit new translation suggestions, and save favourite phrases.

---

### Use Case 1: Register Account
| Element | Description |
| :--- | :--- |
| **ID** | UC‑STU‑01 |
| **Name** | Register Account |
| **Actor** | Student (unregistered user) |
| **Description** | A new user creates an account to access student-specific features. |
| **Preconditions** | User is not logged in. |
| **Postconditions** | A new Student account is created and the user is logged in. |
| **Basic Flow** | 1. User navigates to Registration page.<br>2. User enters email, password, and confirms password.<br>3. System validates input (email format, password strength).<br>4. System creates new user account with role "Student".<br>5. System redirects user to Home page. |
| **Alternative Flows** | 3a. Email already registered → System displays error message.<br>3b. Password too weak → System displays requirements. |
| **Exceptions** | Database connection fails → System displays friendly error page. |
| **Priority** | High |

---

### Use Case 2: Login
| Element | Description |
| :--- | :--- |
| **ID** | UC‑STU‑02 |
| **Name** | Login |
| **Actor** | Student |
| **Description** | A registered student logs in to access personalised features. |
| **Preconditions** | Student has an account. |
| **Postconditions** | Student is authenticated and session is created. |
| **Basic Flow** | 1. User navigates to Login page.<br>2. User enters email and password.<br>3. System validates credentials against ASP.NET Identity.<br>4. System creates session and redirects to Home page. |
| **Alternative Flows** | 3a. Invalid credentials → System displays error message.<br>3b. Account locked → System displays lockout message. |
| **Exceptions** | Database unavailable → System displays friendly error page. |
| **Priority** | High |

---

### Use Case 3: Search Phrases
| Element | Description |
| :--- | :--- |
| **ID** | UC‑STU‑03 |
| **Name** | Search Phrases |
| **Actor** | Student |
| **Description** | A student searches for phrases by keyword or category to find translations. |
| **Preconditions** | User is logged in. Phrase repository has data. |
| **Postconditions** | Search results are displayed with matching language highlighted. |
| **Basic Flow** | 1. Student navigates to Search page.<br>2. Student enters keyword(s) or selects a category.<br>3. System searches across English text and all translations.<br>4. System displays results with matching language highlighted.<br>5. System logs the search (language, category) for statistics. |
| **Alternative Flows** | 2a. No search term entered → System shows all phrases.<br>3a. No results found → System displays "No phrases found" message. |
| **Exceptions** | Search query timeout → System displays "Please try again" message. |
| **Priority** | High |

---

### Use Case 4: View Phrase Details
| Element | Description |
| :--- | :--- |
| **ID** | UC‑STU‑04 |
| **Name** | View Phrase Details |
| **Actor** | Student |
| **Description** | A student views a specific phrase and all its translations. |
| **Preconditions** | Student is logged in. Phrase exists and is approved/active. |
| **Postconditions** | Phrase details and all translations are displayed. Phrase view is logged for statistics. |
| **Basic Flow** | 1. Student clicks on a phrase from search results.<br>2. System displays English text, category, and all approved translations.<br>3. System logs the view (phrase, language viewed). |
| **Alternative Flows** | 1a. Phrase ID invalid → System displays "Phrase not found" error. |
| **Priority** | High |

---

### Use Case 5: Submit Translation Suggestion
| Element | Description |
| :--- | :--- |
| **ID** | UC‑STU‑05 |
| **Name** | Submit Translation Suggestion |
| **Actor** | Student |
| **Description** | A student suggests a translation for a phrase in their language. |
| **Preconditions** | Student is logged in. Phrase exists and is active. |
| **Postconditions** | Translation is saved with "Pending" status. Student's profile shows submission. |
| **Basic Flow** | 1. Student navigates to Phrase Detail page.<br>2. Student clicks "Suggest Translation" button.<br>3. Student selects target language and enters translation text.<br>4. Student submits the form.<br>5. System validates input (language not already translated, text not empty).<br>6. System saves translation with status "Pending".<br>7. System displays confirmation message. |
| **Alternative Flows** | 4a. Translation already exists for that language → System displays warning.<br>5a. Invalid input → System displays validation errors. |
| **Exceptions** | Database save fails → System displays error message. |
| **Priority** | High |

---

### Use Case 6: View Submitted Translations Status
| Element | Description |
| :--- | :--- |
| **ID** | UC‑STU‑06 |
| **Name** | View Submitted Translations Status |
| **Actor** | Student |
| **Description** | A student views their submitted translations and approval status. |
| **Preconditions** | Student is logged in. |
| **Postconditions** | List of submissions with status (Pending/Approved/Rejected) is displayed. |
| **Basic Flow** | 1. Student navigates to their Profile page.<br>2. System displays all translations submitted by the student.<br>3. For each submission, status (Pending, Approved, Rejected) is shown.<br>4. If rejected, the rejection reason is displayed. |
| **Alternative Flows** | 1a. No submissions → System displays "You haven't submitted any translations yet" message. |
| **Priority** | Medium |

---

### Use Case 7: Save Phrase to Favourites
| Element | Description |
| :--- | :--- |
| **ID** | UC‑STU‑07 |
| **Name** | Save Phrase to Favourites |
| **Actor** | Student |
| **Description** | A student saves a phrase to their personal favourites list. |
| **Preconditions** | Student is logged in. Phrase exists and is active. |
| **Postconditions** | Phrase is added to student's favourites (junction table). |
| **Basic Flow** | 1. Student views a phrase (search results or detail page).<br>2. Student clicks "Add to Favourites" button.<br>3. System saves the favourite record.<br>4. Button changes to "Remove from Favourites". |
| **Alternative Flows** | 1a. Phrase already in favourites → System handles gracefully (no duplicate). |
| **Exceptions** | Database save fails → System displays error message. |
| **Priority** | Medium |

---

### Use Case 8: Remove Phrase from Favourites
| Element | Description |
| :--- | :--- |
| **ID** | UC‑STU‑08 |
| **Name** | Remove Phrase from Favourites |
| **Actor** | Student |
| **Description** | A student removes a phrase from their favourites list. |
| **Preconditions** | Student is logged in. Phrase is in favourites. |
| **Postconditions** | Phrase is removed from student's favourites. |
| **Basic Flow** | 1. Student views their Favourites list or a phrase already saved.<br>2. Student clicks "Remove from Favourites" button.<br>3. System deletes the favourite record.<br>4. Button changes to "Add to Favourites". |
| **Exceptions** | Database delete fails → System displays error message. |
| **Priority** | Medium |

---

### Use Case 9: View Favourites List
| Element | Description |
| :--- | :--- |
| **ID** | UC‑STU‑09 |
| **Name** | View Favourites List |
| **Actor** | Student |
| **Description** | A student views all phrases they have saved to favourites. |
| **Preconditions** | Student is logged in. |
| **Postconditions** | List of favourite phrases is displayed. |
| **Basic Flow** | 1. Student navigates to Favourites page.<br>2. System retrieves all phrases saved by the student.<br>3. System displays them in a list/grid format. |
| **Alternative Flows** | 1a. No favourites → System displays "You haven't saved any favourites yet" message. |
| **Priority** | Medium |

---

## 👤 ACTOR: Administrator

**Description:** A privileged user responsible for managing categories/phrases, approving or rejecting student-contributed translations, and viewing usage statistics.

---

### Use Case 10: Approve Translation Submission
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑01 |
| **Name** | Approve Translation Submission |
| **Actor** | Administrator |
| **Description** | Administrator approves a pending student translation, making it visible in search. |
| **Preconditions** | Administrator is logged in. A pending translation exists. |
| **Postconditions** | Translation status changes to "Approved". Translation appears in search results. |
| **Basic Flow** | 1. Administrator navigates to Admin → Pending Submissions.<br>2. System displays list of all pending translations.<br>3. Administrator reviews a submission.<br>4. Administrator clicks "Approve" button.<br>5. System updates status to "Approved".<br>6. Translation becomes visible in public search results. |
| **Alternative Flows** | 4a. Administrator clicks "Reject" → System prompts for reason.<br>4b. Administrator clicks "Request Correction" → System prompts for instructions. |
| **Exceptions** | Database update fails → System displays error message. |
| **Priority** | High |

---

### Use Case 11: Reject Translation Submission
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑02 |
| **Name** | Reject Translation Submission |
| **Actor** | Administrator |
| **Description** | Administrator rejects a pending translation with a reason. |
| **Preconditions** | Administrator is logged in. A pending translation exists. |
| **Postconditions** | Translation status changes to "Rejected". Rejection reason is saved. |
| **Basic Flow** | 1. Administrator navigates to Admin → Pending Submissions.<br>2. System displays list of all pending translations.<br>3. Administrator reviews a submission.<br>4. Administrator clicks "Reject" button.<br>5. System prompts for rejection reason.<br>6. Administrator enters reason and confirms.<br>7. System updates status to "Rejected" and saves reason.<br>8. Student sees rejection reason on their profile. |
| **Exceptions** | Database update fails → System displays error message. |
| **Priority** | High |

---

### Use Case 12: Request Correction on Translation Submission
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑03 |
| **Name** | Request Correction |
| **Actor** | Administrator |
| **Description** | Administrator requests a student to correct their translation suggestion. |
| **Preconditions** | Administrator is logged in. A pending translation exists. |
| **Postconditions** | Submission status remains "Pending". Student is notified with instructions. |
| **Basic Flow** | 1. Administrator navigates to Admin → Pending Submissions.<br>2. System displays list of all pending translations.<br>3. Administrator reviews a submission.<br>4. Administrator clicks "Request Correction" button.<br>5. System prompts for correction instructions.<br>6. Administrator enters instructions and confirms.<br>7. Student sees correction request on their profile. |
| **Exceptions** | Database save fails → System displays error message. |
| **Priority** | Medium |

---

### Use Case 13: Add New Phrase
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑04 |
| **Name** | Add New Phrase |
| **Actor** | Administrator |
| **Description** | Administrator adds a new phrase (English text) to the phrasebook. |
| **Preconditions** | Administrator is logged in. Categories exist. |
| **Postconditions** | New phrase is saved with English text and category. |
| **Basic Flow** | 1. Administrator navigates to Admin → Manage Phrases.<br>2. Administrator clicks "Add Phrase".<br>3. Administrator enters English text and selects category.<br>4. Administrator submits the form.<br>5. System validates input (text not empty, category exists).<br>6. System saves the new phrase. |
| **Alternative Flows** | 3a. Phrase already exists → System displays warning. |
| **Exceptions** | Database save fails → System displays error message. |
| **Priority** | High |

---

### Use Case 14: Edit Existing Phrase
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑05 |
| **Name** | Edit Existing Phrase |
| **Actor** | Administrator |
| **Description** | Administrator edits the English text or category of an existing phrase. |
| **Preconditions** | Administrator is logged in. Phrase exists. |
| **Postconditions** | Phrase details are updated. |
| **Basic Flow** | 1. Administrator navigates to Admin → Manage Phrases.<br>2. Administrator finds the phrase and clicks "Edit".<br>3. System displays edit form with current values.<br>4. Administrator updates text or category.<br>5. Administrator submits the form.<br>6. System validates input and saves changes. |
| **Exceptions** | Database update fails → System displays error message. |
| **Priority** | High |

---

### Use Case 15: Deactivate Phrase
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑06 |
| **Name** | Deactivate Phrase |
| **Actor** | Administrator |
| **Description** | Administrator deactivates an outdated/inappropriate phrase (soft delete). |
| **Preconditions** | Administrator is logged in. Phrase exists and is active. |
| **Postconditions** | Phrase is marked inactive. It no longer appears in search results. |
| **Basic Flow** | 1. Administrator navigates to Admin → Manage Phrases.<br>2. Administrator finds the phrase and clicks "Deactivate".<br>3. System confirms the action.<br>4. Administrator confirms.<br>5. System updates phrase status to "Inactive". |
| **Alternative Flows** | 3a. Administrator cancels → No action taken. |
| **Exceptions** | Database update fails → System displays error message. |
| **Priority** | Medium |

---

### Use Case 16: Reactivate Phrase
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑07 |
| **Name** | Reactivate Phrase |
| **Actor** | Administrator |
| **Description** | Administrator reactivates a previously deactivated phrase. |
| **Preconditions** | Administrator is logged in. Phrase exists and is inactive. |
| **Postconditions** | Phrase is marked active. It reappears in search results. |
| **Basic Flow** | 1. Administrator navigates to Admin → Manage Phrases (show inactive).<br>2. Administrator finds the phrase and clicks "Reactivate".<br>3. System updates phrase status to "Active". |
| **Exceptions** | Database update fails → System displays error message. |
| **Priority** | Low |

---

### Use Case 17: Add Category
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑08 |
| **Name** | Add Category |
| **Actor** | Administrator |
| **Description** | Administrator adds a new category for organising phrases. |
| **Preconditions** | Administrator is logged in. |
| **Postconditions** | New category is saved. |
| **Basic Flow** | 1. Administrator navigates to Admin → Manage Categories.<br>2. Administrator clicks "Add Category".<br>3. Administrator enters category name and description.<br>4. Administrator submits the form.<br>5. System validates input (name not empty, unique).<br>6. System saves the new category. |
| **Exceptions** | Database save fails → System displays error message. |
| **Priority** | High |

---

### Use Case 18: Edit Category
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑09 |
| **Name** | Edit Category |
| **Actor** | Administrator |
| **Description** | Administrator edits an existing category name or description. |
| **Preconditions** | Administrator is logged in. Category exists. |
| **Postconditions** | Category details are updated. |
| **Basic Flow** | 1. Administrator navigates to Admin → Manage Categories.<br>2. Administrator finds category and clicks "Edit".<br>3. System displays edit form with current values.<br>4. Administrator updates name or description.<br>5. Administrator submits the form.<br>6. System validates and saves changes. |
| **Exceptions** | Database update fails → System displays error message. |
| **Priority** | High |

---

### Use Case 19: Deactivate Category
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑10 |
| **Name** | Deactivate Category |
| **Actor** | Administrator |
| **Description** | Administrator deactivates a category (soft delete). |
| **Preconditions** | Administrator is logged in. Category exists. Category has no active phrases? |
| **Postconditions** | Category is marked inactive. |
| **Basic Flow** | 1. Administrator navigates to Admin → Manage Categories.<br>2. Administrator finds category and clicks "Deactivate".<br>3. System confirms the action.<br>4. Administrator confirms.<br>5. System updates category status to "Inactive". |
| **Exceptions** | Database update fails → System displays error message. |
| **Priority** | Medium |

---

### Use Case 20: View Language Usage Report
| Element | Description |
| :--- | :--- |
| **ID** | UC‑ADM‑11 |
| **Name** | View Language Usage Report |
| **Actor** | Administrator |
| **Description** | Administrator views statistics on which languages and categories are most searched/viewed. |
| **Preconditions** | Administrator is logged in. Statistics have been logged. |
| **Postconditions** | Report is displayed with charts/tables. |
| **Basic Flow** | 1. Administrator navigates to Admin → Statistics.<br>2. System retrieves aggregated usage data.<br>3. System displays: most searched languages, most viewed categories, popular phrases.<br>4. Administrator uses this data to prioritise translation efforts. |
| **Alternative Flows** | 1a. No statistics yet → System displays "No data available" message. |
| **Priority** | Medium |

---

## 📊 Use Case Summary Table

| ID | Use Case Name | Actor | Priority |
| :--- | :--- | :--- | :--- |
| UC‑STU‑01 | Register Account | Student | High |
| UC‑STU‑02 | Login | Student | High |
| UC‑STU‑03 | Search Phrases | Student | High |
| UC‑STU‑04 | View Phrase Details | Student | High |
| UC‑STU‑05 | Submit Translation Suggestion | Student | High |
| UC‑STU‑06 | View Submitted Translations Status | Student | Medium |
| UC‑STU‑07 | Save Phrase to Favourites | Student | Medium |
| UC‑STU‑08 | Remove Phrase from Favourites | Student | Medium |
| UC‑STU‑09 | View Favourites List | Student | Medium |
| UC‑ADM‑01 | Approve Translation Submission | Administrator | High |
| UC‑ADM‑02 | Reject Translation Submission | Administrator | High |
| UC‑ADM‑03 | Request Correction | Administrator | Medium |
| UC‑ADM‑04 | Add New Phrase | Administrator | High |
| UC‑ADM‑05 | Edit Existing Phrase | Administrator | High |
| UC‑ADM‑06 | Deactivate Phrase | Administrator | Medium |
| UC‑ADM‑07 | Reactivate Phrase | Administrator | Low |
| UC‑ADM‑08 | Add Category | Administrator | High |
| UC‑ADM‑09 | Edit Category | Administrator | High |
| UC‑ADM‑10 | Deactivate Category | Administrator | Medium |
| UC‑ADM‑11 | View Language Usage Report | Administrator | Medium |