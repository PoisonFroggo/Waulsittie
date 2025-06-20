import requests
from bs4 import BeautifulSoup

def render_grid_pub_doc(url):
    if not url.startswith("https://docs.google.com/document/d/e/"):
        raise ValueError("URL must be a published Google Docs URL (starting with /d/e/...)")

    # Fetch the published HTML
    response = requests.get(url)
    response.raise_for_status()

    soup = BeautifulSoup(response.text, "html.parser")

    # Find all table rows (assumes a single table)
    rows = soup.find_all("tr")

    entries = []

    # Skip header row; parse remaining
    for row in rows[1:]:
        cells = row.find_all("td")
        if len(cells) != 3:
            continue
        try:
            x = int(cells[0].get_text(strip=True))
            char = cells[1].get_text(strip=True)
            y = int(cells[2].get_text(strip=True))
            entries.append((x, y, char))
        except ValueError:
            continue  # skip any row with invalid data

    if not entries:
        print("No valid data found in the document.")
        return

    # Determine grid size
    max_x = max(x for x, _, _ in entries)
    max_y = max(y for _, y, _ in entries)

    # Create grid filled with spaces
    grid = [[" " for _ in range(max_x + 1)] for _ in range(max_y + 1)]

    # Populate the grid
    for x, y, char in entries:
        grid[y][x] = char

    # Print the grid
    for row in grid:
        print("".join(row))

render_grid_pub_doc(
    "https://docs.google.com/document/d/e/2PACX-1vQGUck9HIFCyezsrBSnmENk5ieJuYwpt7YHYEzeNJkIb9OSDdx-ov2nRNReKQyey-cwJOoEKUhLmN9z/pub"
)