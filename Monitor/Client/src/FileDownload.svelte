<script lang="ts">
  import DirectoryTree from './lib/DirectoryTree.svelte';
  import Nav from './Nav.svelte';

  const treePromise = fetch("/api/video_tree")
    .then(response => response.json());

</script>

<Nav></Nav>
<main>
  {#await treePromise}
    Loading...
  {:then directoryTree} 
    <DirectoryTree tree={directoryTree} collapsed={false}></DirectoryTree>
  {/await}
</main>

<style>
  main {
    display: flex;
    justify-content: left;
    align-items: top;
    margin: 0;
    width: 100%;
    height: 100%;
  }
</style>
