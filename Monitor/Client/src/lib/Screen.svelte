<script>
    import { createImage } from "./stores";
    export let src;

    const image = createImage(src);
</script>

<div class=image-container>
    {#if $image.imageUrl}
        <img src={$image.imageUrl} alt="Loading...">
        <p class=timestamp>{$image.timestamp}</p>
    {:else}
        <p class=alt-text>Loading...</p>
    {/if}
</div>

<style>
    .image-container {
        position: relative;
        width: 480px;
        height: 272px;
        text-align: center;
        border-radius: 10px;
        background-color: black;
    }

    img {
        position: absolute;
        left: 0;
        top: 0;
        width: 480px;
        height: 272px;
        line-height: 200px;
        text-align: center;
    }

    .timestamp {
        position: absolute;
        right: 10px;
        bottom: 5px;
        border-radius: 5px;
        background: #1a1a1a8f;
    }

    .alt-text {
        position: absolute;
        width: 100px;
        height:25px;
        top: 50%;
        left: 50%;
        text-align: center;
        line-height: 25px;
        margin: -12.5px 0 0 -50px;
        padding: 0;
    }
</style>