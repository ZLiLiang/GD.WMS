<template>
  <!-- <canvas ref="qrcode"/> -->
  <img ref="qrcode"/>
</template>
  
<script setup>
import QRCode from 'qrcode'
const props = defineProps({
  value: {
    type: String,
    default: "https://baidu.com"
  }
})

const qrcode = ref(null)

var opts = {
  errorCorrectionLevel: 'H',
  type: 'image/jpeg',
  quality: 0.3,
  width: 120,
  margin: 3,
  // color: {
  //   dark: "#010599FF",
  //   light: "#FFBF60FF"
  // }
}


onMounted(() => {
  render()
})

const render = () => {
  // QRCode.toCanvas(qrcode.value, props.value, (error) => {
  //   if (error) {
  //     console.log(error)
  //   }
  // })
  QRCode.toDataURL(props.value, opts, function (err, url) {
  if (err) {
    console.log(err);
  }

  qrcode.value.src = url
})
}
</script>