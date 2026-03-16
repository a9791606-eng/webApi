# 🍦 Ice Cream Shop API

API ויישום ווב לניהול חנות גלידה עם תמיכה במשתמשים, עובדים וניהול פריטים בזמן אמת.

## 🎯 תכונות עיקריות

### 👤 ניהול משתמשים
- הרשמה (Signup) - משתמשים חדשים יכולים להירשם בשם משתמש, אימייל וסיסמה
- התחברות (Login) - התחברות עם שם משתמש וסיסמה
- Auth עם JWT tokens - אבטחה מלאה
- אפשרות להתחברות דרך Google OAuth (optional)

### 🍦 ניהול גלידות
- רשימת גלידות כוללות
- הוספה, עריכה ומחיקה של פריטים
- סימון "ללא גלוטן"
- תמונות וכרטיסים יפים

### 👥 ניהול עובדים
- הצגת רשימת עובדים
- נתונים מפורטים (שם, אימייל, סניף וכד')
- ניהול (עריכה/מחיקה - לנהלים בלבד)

### 📡 תכונות בזמן אמת
- SignalR - עדכונים חיים של פעילויות
- Activity Feed - ההתראות החדשות ביותר
- WebSocket Support - תקשורת דו-כיוונית

### 📝 תיעוד
- Swagger/OpenAPI documentation
- API endpoints מתועדים

## 🛠️ טכנולוגיות

**Backend:**
- C# / ASP.NET Core 8.0
- Entity Framework (Optional)
- JWT Authentication
- SignalR הי בזמן אמת
- Serilog - Logging

**Frontend:**
- HTML5 / CSS3
- JavaScript (Vanilla)
- Responsive Design
- LocalStorage / SessionStorage

**Database:**
- JSON Files (Users.json, IceCream.json)

## 🚀 התחלה מהירה

### דרישות מוקדמות
- .NET 8.0 SDK
- Node.js (אופציונלי)
- דפדפן מודרני

### התקנה

```bash
# Clone and navigate
cd webApi/Ice\ cream\ project

# הרץ את ה-API
dotnet run
```

**הגש ל:** `https://localhost:7274`

## 📁 מבנה הפרויקט

```
webApi/
├── Ice cream project/
│   ├── Controllers/          # API Endpoints
│   │   ├── IceCreamController.cs
│   │   ├── UsersController.cs
│   │   ├── LoginController.cs
│   │   └── ExternalAuthController.cs
│   ├── Models/               # Data Models
│   │   ├── User.cs
│   │   ├── IceCream.cs
│   │   └── LogEntry.cs
│   ├── Services/             # Business Logic
│   │   ├── IceCreamService.cs
│   │   ├── UsersService.cs
│   │   ├── UserRepository.cs
│   │   └── RabbitMqService.cs
│   ├── Interfaces/           # Contracts
│   ├── Middleware/           # Custom Middleware
│   ├── Hubs/                 # SignalR Hubs
│   ├── wwwroot/              # Static Files
│   │   ├── index.html        # דף הגלידות
│   │   ├── login.html        # דף התחברות
│   │   ├── signup.html       # דף הרשמה
│   │   ├── user.html         # דף העובדים
│   │   ├── css/
│   │   │   └── site.css
│   │   └── js/
│   │       ├── site.js
│   │       ├── login.js
│   │       └── signup.js
│   ├── Data/
│   │   ├── Users.json        # משתמשים
│   │   └── IceCream.json     # גלידות
│   ├── Program.cs            # Startup Configuration
│   └── appsettings.json      # Configuration

```

## 🔑 משתמשים ברירת מחדל

```json
[
  {
    "Username": "admin",
    "Password": "admin",
    "IsAdmin": true
  },
  {
    "Username": "user1",
    "Password": "userpass",
    "IsAdmin": false
  }
]
```

## 📡 API Endpoints

### Users
- `POST /users/signup` - הרשמה (ללא אימות)
- `POST /login` - התחברות
- `GET /users` - רשימת משתמשים (צריך לוגו)
- `POST /users` - הוסף משתמש (Admin בלבד)

### Ice Cream
- `GET /icecream` - רשימה
- `POST /icecream` - הוסף
- `PUT /icecream/{id}` - עדכן
- `DELETE /icecream/{id}` - מחק

### Activity
- `GET /activityHub` - SignalR Hub לעדכונים בזמן אמת

## 🔐 אבטחה

- JWT Tokens עם sessionStorage
- Admin-only endpoints מוגנים
- Password validation
- CORS enabled
- HTTPS תמיכה

## 🎨 עיצוב

- **Pastel Summer Theme** - צבעים חם וידידותיים
- **Responsive Design** - עובד על כל גדלי מסך
- **Accessibility** - תמיכה בנושים ו-ARIA labels
- **Right-to-Left Support** - עברית מלאה

## 🔄 עדכונים בזמן אמת

SignalR משמש לעדכונים חיים:
- Activity Feed עם שינויים חדשים
- תמיכה בחיבורים מרובים
- Automatic Reconnection

## 📝 הערות

- משתמשים חדשים מצטרפים ל-`Users.json` אוטומטית
- גלידות חדשות נשמרות ב-`IceCream.json`
- כל הפעולות עם timestamp logging
- Google OAuth הוא אופציונלי - בדוק את `appsettings.json`

## 🚧 עתידות

- [ ] Database Integration (SQL Server / PostgreSQL)
- [ ] Mobile App (React Native)
- [ ] Payment Integration
- [ ] Email Notifications
- [ ] Advanced Filtering & Search

## 📧 יצירת קשר

לשאלות או הצעות, צור Issue בפרויקט.

---

**שנת 2026** | Made with ❤️ for the Ice Cream Shop
