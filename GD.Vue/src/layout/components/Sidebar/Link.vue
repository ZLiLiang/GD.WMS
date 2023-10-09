<template>
    <component :is="router-link" v-bind="linkProps()">
        <slot />
    </component>
</template>

<script setup>
const props = defineProps({
    to: {
        type: [String, Object],
        required: true,
    },
})

function linkProps() {
    return {
        to: props.to,
    }
}
</script>