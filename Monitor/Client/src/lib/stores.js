import { readable } from 'svelte/store';
import { getLatestImageUrl } from './apiClient';



/**
 * @param {string} address
 */
export function createImage(address) {
    return readable(null, function start(set) {
        const interval = setInterval(() => {
            getLatestImageUrl(address)
                .then(url => set(url))
        }, 1000);


        return function stop() {
            clearInterval(interval);
        }
    });
}