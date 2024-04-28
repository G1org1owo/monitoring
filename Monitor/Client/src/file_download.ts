import './app.css'
import 'bootstrap-icons/font/bootstrap-icons.css'
import FileDownload from './FileDownload.svelte'

const app = new FileDownload({
  target: document.getElementById('app'),
})

export default app
