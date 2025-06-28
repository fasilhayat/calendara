# 
# 🗓️ Calendara

**Calendara** is a lightweight holiday calendar service that provides official holidays and bank closing days for **Denmark**, **Sweden**, and **Norway**. Whether you're scheduling work, planning deployments, or syncing organizational calendars, MythosCalendar ensures you're always aware of when the Nordics rest.

## ✨ Features

- 📅 Up-to-date lists of holidays and bank closing days
- 🌍 Covers **Denmark**, **Sweden**, and **Norway**
- 🔄 JSON-based API ready for integration
- 🕰️ Rooted in the idea of timeless cycles and calendar wisdom from mythology

## 📌 Supported Countries

| Country  | Holidays | Bank Closing Days |
|----------|----------|--------------------|
| 🇩🇰 Denmark | ✅       | ✅                 |
| 🇸🇪 Sweden | ✅       | ✅                 |
| 🇳🇴 Norway | ✅       | ✅                 |

## 📂 Example Usage

You can request a holiday list via the API (example below):

```http
GET /api/holidays/2025/DK
