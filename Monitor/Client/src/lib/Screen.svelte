<script>
    import { createImage } from "./stores";
    import { createEventDispatcher } from 'svelte';
    export let target;
    export let size;

    const dispatch = createEventDispatcher();

    function notifyConnectionLost() {
        dispatch("connectionLost", {});
    }

    const image = createImage(target);

    let hidden = true;

    $: if($image.error && $image.info === "Connection Lost") {
        notifyConnectionLost();
    }
</script>

<!-- svelte-ignore a11y-no-static-element-interactions -->
<div class=image-container style="width: {size*16}px; height: {size*9}px" on:mouseenter={() => hidden = false} on:mouseleave={() => hidden = true}>
    {#if !$image.error}
        <img src={$image.imageUrl} alt="Loading...">
        <p hidden={hidden} class=timestamp style="font-size: {size/2}px">{new Date($image.timestamp).toUTCString()}</p>
    {:else}
        <p class=alt-text>Loading...</p>
    {/if}
</div>

<style>
    .image-container {
        display: flex;
        align-items: center;
        justify-content: center;
        position: relative;
        text-align: center;
        border-radius: 10px;
        background-color: black;
    }

    img {
        position: absolute;
        left: 0;
        top: 0;
        width: 100%;
        height: 100%;
        border-radius: 10px;
        text-align: center;
    }

    .timestamp {
        position: absolute;
        right: 2%;
        bottom: 2%;
        border-radius: 5%;
        background: #1a1a1a8f;
        margin: 0;
    }

    .alt-text {
        text-align: center;
        margin: 0;
        padding: 0;
    }
</style>