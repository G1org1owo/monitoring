<script>
    export let tree;
    export let collapsed = true;

    function getFileName(path) {
        return path.replace(/^.*[\\\/]/, '');
    }

    function formatDirectoryName(fileName) {
        return fileName.endsWith('/')? fileName : fileName + "/";
    }

    function isLastFolderInTree(folder) {
        return !folder.children.some(element => element.type === 'directory');
    }

    $: console.log("tree:", tree);
</script>

<div class='entry'>
    {#if tree.type == 'file'}
        <a href={tree.name}>{getFileName(tree.name)}</a>
    {:else if tree.type == 'directory'}
        <!-- svelte-ignore a11y-click-events-have-key-events -->
        <!-- svelte-ignore a11y-no-noninteractive-element-interactions -->
        <p on:click={() => collapsed = !collapsed}>
            <i class="bi {collapsed? "bi-plus-lg" : "bi-dash-lg"}"></i>            

            {formatDirectoryName(getFileName(tree.name))}

            {#if isLastFolderInTree(tree)}
                <a href="./video/{encodeURIComponent(tree.name)}"><i class="bi bi-download"></i></a>
            {/if}
        </p>

        {#if !collapsed}
            <ul>
                {#each tree.children as node}
                    <li>
                        <svelte:self tree={node}></svelte:self>
                    </li>
                {/each}    
            </ul>
        {/if}
    {/if}    
</div>

<style>
    .entry {
        text-align: left;
        /* margin: 0 0 0 20px; */
    }

    ul {
        margin: 0;
    }

    li {
        list-style: none;
    }
    
    p {
        font-weight: 500;
        /* color: #646cff; */
        text-decoration: inherit;
        margin: 0;
    }
</style>