<template>
    <div>
        <!-- <canvas ref="barcodeRef" /> -->
        <img ref="barcodeRef" />
    </div>
</template>
  
<script setup>
import JsBarcode from 'jsbarcode'
const props = defineProps({
    value: [String, Number],
    //选择要使用的条形码类型
    format: {
        type: [String],
        default: "CODE39"
    },
    //设置条之间的宽度
    width: {
        type: [String, Number],
        default: 0.8
    },
    height: {
        type: [String, Number],
        default: 85
    },
    //覆盖显示的文本
    text: [String, Number],
    //使文字加粗体或变斜体
    fontOptions: {
        type: [String],
        default: "bold italic"
    },
    //设置文本的字体
    font: [String, Number],
    //设置文本的水平对齐方式
    textAlign: [String],
    //设置文本的垂直位置
    textPosition: [String],
    //设置条形码和文本之间的间距
    textMargin: [String, Number],
    //设置文本的大小
    fontSize: {
        type: [String, Number],
        default: 15
    },
    //设置条和文本的颜色
    lineColor: [String],
    //设置条形码的背景
    background: {
        type: [String],
        default: "#ffffff"
    },
    //设置条形码周围的空白边距
    margin: [String, Number],
    // 是否在条形码下方显示文字
    displayValue: {
        type: [String, Boolean],
        default: true
    }
})

const settings = {
    format: props.format,//选择要使用的条形码类型
    width: props.width,//设置条之间的宽度
    height: props.height,//高度
    displayValue: props.displayValue,//是否在条形码下方显示文字
    text: props.text,//覆盖显示的文本
    fontOptions: props.fontOptions,//使文字加粗体或变斜体
    font: props.font,//设置文本的字体
    textAlign: props.textAlign,//设置文本的水平对齐方式
    textPosition: props.textPosition,//设置文本的垂直位置
    textMargin: props.textMargin,//设置条形码和文本之间的间距
    fontSize: props.fontSize,//设置文本的大小
    background: props.background,//设置条形码的背景
    lineColor: props.lineColor,//设置条和文本的颜色。
    margin: props.margin//设置条形码周围的空白边距
}

const removeUndefinedProps = (obj) => {
    for (let prop in obj) {
        if (obj.hasOwnProperty(prop) && obj[prop] === undefined) {
            delete obj[prop]
        }
    }
}

const barcodeRef = ref(null)

onMounted(() => {
    removeUndefinedProps(settings)
    render()
})

const render = () => {
    JsBarcode(barcodeRef.value, props.value, settings)
}
</script>