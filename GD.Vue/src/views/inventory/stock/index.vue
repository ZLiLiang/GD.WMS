<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="联系方式" prop="spuName">
                <el-input v-model="queryParams.spuName" placeholder="请输入商品名称" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="创建时间">
                <el-date-picker v-model="dateRange" style="width: 300px" type="daterange" range-separator="-"
                    start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                <el-button icon="refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
        </el-form>

        <!-- 工具栏 -->
        <el-row :gutter="10">
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="loading" :data="stockList" highlight-current-row>
            <el-table-column label="商品编码" prop="spuCode" />
            <el-table-column label="商品名称" prop="spuName" />
            <el-table-column label="规格编码" prop="skuCode" />
            <el-table-column label="数量" prop="qty" />
            <el-table-column label="可用数量" prop="availableQty" />
            <el-table-column label="锁定数量" prop="lockedQty" />
            <el-table-column label="冻结数量" prop="frozenQty" />
            <el-table-column label="到货通知书数量" prop="asnQty" />
            <el-table-column label="待卸货数量" prop="toUnloadQty" />
            <el-table-column label="待分拣数量" prop="toSortQty" />
            <el-table-column label="已分拣数量" prop="sortedQty" />
            <el-table-column label="欠货数量" prop="shortageQty" />
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />
    </div>
</template>

<script setup>
import { getStock, exportStock } from "@/api/inventory/stock";

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 供应商列表
const stockList = ref([])
// 时间范围
const dateRange = ref([])
// 数据
const data = reactive({
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        spuName: undefined
    }
})
// 表单、搜索参数、规则
const { queryParams } = toRefs(data)
// 当时实例
const { proxy } = getCurrentInstance()

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getStock(params).then(res => {
        loading.value = false
        stockList.value = res.data.result
        total.value = res.data.totalNum
    })
}

/**
 * 搜索按钮操作
 */
function handleQuery() {
    queryParams.pageNum = 1
    getList()
}

/**
 * 重置按钮操作
 */
function resetQuery() {
    dateRange.value = []
    proxy.resetForm('queryRef')
    handleQuery()
}

/**
 * 导出按钮操作 
 */
function handleExport() {
    proxy
        .$modal
        .confirm('是否确认导出所有库存列表数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportStock()
        })
}

getList()
</script>

<style></style>