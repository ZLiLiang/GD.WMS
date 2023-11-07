<template>
    <div class="app-container">
        <!-- 搜索表单 -->
        <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch">
            <el-form-item label="商品编码" prop="commoditycode">
                <el-input v-model="queryParams.commoditycode" placeholder="请输入商品编码" clearable />
            </el-form-item>
            <el-form-item label="商品名称" prop="commodityName">
                <el-input v-model="queryParams.commodityName" placeholder="请输入商品名称" clearable />
            </el-form-item>
            <el-form-item label="商品类别" prop="categoryName">
                <el-input v-model="queryParams.categoryName" placeholder="请输入商品类别" clearable />
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                <el-button icon="refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
        </el-form>

        <el-row :gutter="10" class="mb8">
            <el-col :span="1.5">
                <el-button type="primary" plain icon="Plus" @click="handleAdd">新增</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleAdd">导出</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button color="#626aef" plain @click="handleAdd">
                    <el-icon>
                        <SvgIcon name="qrcode"/>
                    </el-icon>
                    <span> 二维码 </span>
                </el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button color="#ff6a6a" plain @click="handleAdd">
                    <el-icon>
                        <SvgIcon name="barcode"/>
                    </el-icon>
                    <span> 条形码 </span>
                </el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="info" plain icon="Sort" @click="toggleExpandAll">展开/折叠</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>
    </div>
</template>

<script setup>
const showSearch = ref(false)
const commodityQueryOptions = ref([])
const data = reactive({
    form: {},
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        commodityName: undefined,
        commoditycode: undefined,
        categoryName: undefined
    },
    rules: {}
})

const { form, queryParams, rules } = toRefs(data)

/** 查询商品类别列表 */
function getList() {

}

/** 新增按钮操作 */
function handleAdd(row) {
    // reset()
    // getTreeselect()
    // if (row != null && row.menuId != undefined) {
    //     form.value.parentId = row.menuId
    // } else {
    //     form.value.parentId = 0
    // }
    // open.value = true
    // title.value = "新增"
}

/** 展开/折叠操作 */
function toggleExpandAll() {
    // isExpandAll.value = !isExpandAll.value
    // const $table = listRef.value
    // if ($table) {
    //     if (isExpandAll.value) {
    //         $table.setAllTreeExpand(true)
    //     } else {
    //         $table.clearTreeExpand()
    //     }
    // }
}

</script>

<style lang="scss">

</style>