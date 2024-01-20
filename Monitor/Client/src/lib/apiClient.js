/**
 * @param {string} [url]
 */
export async function getLatestImageUrl(url) {
    return fetch(url)
        .then(response => response.json())
        .then(json => json.url);
}