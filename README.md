
VintedTracker
=============

VintedTracker is a lightweight, configurable .NET 8 application that monitors item listings on Vinted.pl
based on custom search parameters and sends new results directly to Telegram using a bot.

The project uses headless Chrome (via PuppeteerSharp) to obtain session tokens and the Vinted API v2
to fetch results. Notifications are sent to Telegram chats with item details and direct links.

--------------------------------------------------------------------------------

FEATURES
--------

- Configurable item trackers (via JSON)
- Independent async workers for each tracker
- Prevents duplicate notifications (in-memory)
- Sends item image, title, brand, size, and price to Telegram
- Simple architecture – no database or frontend

--------------------------------------------------------------------------------

PREREQUISITES
-------------

- .NET 8 SDK
- Telegram bot (via @BotFather)
- Your Telegram chat ID(s)
- Chrome for Testing – required for PuppeteerSharp

--------------------------------------------------------------------------------

GETTING STARTED
---------------

1. Clone the repository:
   git clone https://github.com/your-username/VintedTracker.git
   cd VintedTracker

2. Download Chrome for Testing from:
   https://googlechromelabs.github.io/chrome-for-testing/
   Extract it, and then set the full binary path in Program.cs:
   var chromePath = "/Users/your-name/Downloads/chrome-mac-arm64/Google Chrome for Testing.app/Contents/MacOS/Google Chrome for Testing";

--------------------------------------------------------------------------------

CONFIGURATION
-------------

Config/TelegramConfig.json
(This file is ignored in .gitignore. Do not push it to public repositories.)

{
  "BotToken": "YOUR_TELEGRAM_BOT_TOKEN",
  "ChatIds": ["123456789"]
}

Config/TrackerConfig.json
You can define multiple trackers, each with its own search URL and interval:

[
  {
    "title": "Nike shoes",
    "url": "https://www.vinted.pl/api/v2/catalog/items?search_text=nike&size_id[]=44&price_to=200",
    "intervalSeconds": 30
  },
  {
    "title": "Jeans",
    "url": "https://www.vinted.pl/api/v2/catalog/items?search_text=jeans&price_to=100",
    "intervalSeconds": 60
  }
]

You can get these URLs from the network tab in DevTools (items endpoint). Make sure to copy the actual API request.

--------------------------------------------------------------------------------

RUNNING THE APP
---------------

dotnet run

You will see output in the console such as:

  Starting tracker for: Nike shoes
  Nike shoes – Initial items loaded: 5
  Checking Nike shoes...
  Nike shoes – 2 new items sent to Telegram.

--------------------------------------------------------------------------------

NOTES
-----

- TelegramConfig.json is listed in .gitignore. Push a placeholder version if needed.
- Session refresh is automated every 20 minutes.
- Each tracker runs independently with its own in-memory item history.

--------------------------------------------------------------------------------

LICENSE
-------

MIT © 2025 @mkwkamil
