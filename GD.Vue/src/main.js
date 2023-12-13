import { createApp } from 'vue'

import '@/assets/styles/index.scss' // global css
// 注册指令
import plugins from './plugins' // plugins
import App from '@/App.vue'
import router from '@/router/index'
import pinia from '@/store/index'
import vxetb from './vxe-tb'

// svg图标
import '@/assets/iconfont/iconfont' //iconfont
import 'virtual:svg-icons-register'
import SvgIcon from '@/components/SvgIcon/index.vue'
import elementIcons from '@/components/SvgIcon/svgicon'
import { parseTime, resetForm, addDateRange, handleTree, selectDictLabel, download } from '@/utils/ruoyi'
import './permission' // permission control

// 自定义表格工具组件
import RightToolbar from '@/components/RightToolbar'
// 分页组件
import Pagination from '@/components/Pagination'
// Dialog组件
import Dialog from '@/components/Dialog'
// 二维码组件
import QrBarCode from '@/components/QrBarCode'
// 商品选择框
import SkuSelect from '@/components/SkuSelect'
// 库位选择框
import LocationSelect from '@/components/LocationSelect'
// 库存选择框
import StockSelect from '@/components/StockSelect'

const app = createApp(App)

app.config.globalProperties.addDateRange = addDateRange
app.config.globalProperties.resetForm = resetForm

// 全局组件挂载
app.component('Pagination', Pagination)
app.component('RightToolbar', RightToolbar)
app.component('svg-icon', SvgIcon)
app.component('zDialog', Dialog)
app.component('zQrBarDialog', QrBarCode)
app.component('zSkuSelectDialog', SkuSelect)
app.component('zLocationSelectDialog', LocationSelect)
app.component('zStockSelectDialog', StockSelect)

app.use(pinia)
app.use(router)
app.use(vxetb)
app.use(elementIcons)
app.use(plugins)
app.mount('#app')