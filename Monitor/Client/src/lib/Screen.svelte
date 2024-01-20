<script>
    import { createImage } from "./stores";
    export let src;

    const image = createImage(src);
</script>

<div class=image-container>
    <img src={$image.imageUrl} alt="Loading...">
    <p class=timestamp>{$image.timestamp}</p>
</div>

<style>
    img {
        width: 480px;
        height: 272px;
        border-radius: 10px;
    }
</style>