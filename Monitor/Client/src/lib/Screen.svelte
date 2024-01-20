<script>
    import { createImage } from "./stores";
    export let src;

    const image = createImage(src);
</script>

<img src={$image} alt="Loading...">

<style>
    img {
        width: 480px;
        height: 272px;
        border-radius: 10px;
    }
</style>