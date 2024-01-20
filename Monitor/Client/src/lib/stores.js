import { readable } from 'svelte/store';
import { getLatestImage } from './apiClient';



/**
 * @param {string} address
 */
export function createImage(address) {
    return readable({imageUrl:"", timestamp:""}, function start(set) {
        const interval = setInterval(() => {
            getLatestImage(address)
                .then(image => set(image))
        }, 1000);


        return function stop() {
            clearInterval(interval);
        }
    });
}