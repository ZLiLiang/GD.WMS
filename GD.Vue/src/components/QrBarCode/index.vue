<template>
    <el-dialog title="预览" v-model="isOpen" width="900px" :show-close="false" :draggable="true"
        :close-on-click-modal="false">
        <div class="qr-bar-code" id='printCode'>
            <div v-for="item in props.codeValues" class="qr-bar-code-box">
                <div style="height: 60%;  text-align: center;">
                    <qrcode :value="JSON.stringify(item)" v-if="isShow" />
                    <barcode :value="item.skucode" v-else />
                </div>
                <div style="height: 40%; text-align: center;">
                    <slot :item="item"></slot>
                </div>
            </div>
        </div>
        <template #footer>
            <el-button text @click="cancel">取消</el-button>
            <el-button type="primary" @click="codePrint">打印</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import qrcode from './qrcode.vue';
import barcode from './barcode.vue';
import printJS from 'print-js'

const props = defineProps({
    visible: {
        type: Boolean,
        default: false
    },
    codeValues: {
        type: Array,
        default: []
    },
    codeMode: {
        type: String,
        default: 'qrcode'
    }
})

const emit = defineEmits(["update:visible"]);

const isShow = computed(() => props.codeMode === 'qrcode');
const isOpen = ref(false)

watch(
    () => props.visible,
    (val) => {
        isOpen.value = val
    },
    { deep: true, immediate: true }
)

function cancel() {
    emit("update:visible", false)
}

function codePrint() {
    console.log("打印！！！");
    printJS({
        printable: 'printCode',// 标签元素id
        type: 'html',
        // header: '',
        // targetStyles: ['*'],
        style: 'div {flex-wrap: wrap;}'
    })
    //各个配置项
    //printable:要打印的id。
    //type:可以是 html 、pdf、 json 等。
    //properties:是打印json时所需要的数据属性。
    //gridHeaderStyle和gridStyle都是打印json时可选的样式。
    //repeatTableHeader:在打印JSON数据时使用。设置为时false，数据表标题将仅在第一页显示。
    //scanStyles:设置为false时，库将不处理应用于正在打印的html的样式。使用css参数时很有用，此时自己设置的原来想要打印的样式就会失效，在打印预览时可以看到效果
    //targetStyles: [’*’],这样设置继承了页面要打印元素原有的css属性。
    //style:传入自定义样式的字符串，使用在要打印的html页面 也就是纸上的样子。
    //ignoreElements：传入要打印的div中的子元素id，使其不打印。非常好用
}
</script>

<style  lang="scss" scoped>
.qr-bar-code {
    display: flex;
    flex-wrap: wrap; //换行
    align-content: flex-start; //紧揍排列
    overflow: auto;
    background-color: rgb(239, 239, 239);
    width: 100%;
    height: 350px;
}

.qr-bar-code-box {
    width: 200px;
    height: 200px;
    margin: 5px;
    background-color: bisque;
}
</style>