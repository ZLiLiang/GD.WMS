<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="供应商名称" prop="supplierName">
                <el-input v-model="queryParams.supplierName" placeholder="请输入供应商名称" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="规格名称" prop="skuName">
                <el-input v-model="queryParams.skuName" placeholder="请输入规格名称" clearable style="width: 160px" />
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
        <el-table v-loading="loading" :data="arriveList" highlight-current-row>
            <el-table-column label="到货通知书编号" prop="asnNo" />
            <el-table-column label="商品编码" prop="spuCode" />
            <el-table-column label="商品名称" prop="spuName" />
            <el-table-column label="规格编码" prop="skuCode" />
            <el-table-column label="规格名称" prop="skuName" />
            <el-table-column label="货主名称" prop="ownerName" />
            <el-table-column label="供应商名称" prop="supplierName" />
            <el-table-column label="到货通知书数据" prop="asnQty" />
            <el-table-column label="总重量" prop="weight" />
            <el-table-column label="总体积" prop="volume" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />
            <el-table-column label="操作人" prop="updateBy" v-if="columns.showColumn('updateBy')" />
            <el-table-column label="操作时间" prop="updateTime" v-if="columns.showColumn('updateTime')" />
            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <el-button text icon="check" title="编辑" @click.stop="handleUpdate(scope.row)" />
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

    </div>
</template>

<script setup>
import { getAllInfo, confirmArrive, exportAllInfo } from '@/api/receive/arrive'

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 供应商列表
const arriveList = ref([])
// 展示对话框
const open = ref(false)
// 时间范围
const dateRange = ref([])
// 数据
const data = reactive({
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        supplierName: undefined,
        skuName: undefined,
        asnStatus: 0
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 1, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 2, label: `操作人`, visible: false, prop: 'updateBy' },
    { key: 3, label: `操作时间`, visible: false, prop: 'updateTime' },
    { key: 4, label: `操作`, visible: true, prop: 'operate' }
])
// 搜索参数
const { queryParams } = toRefs(data)
// 当时实例
const { proxy } = getCurrentInstance()

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        arriveList.value = res.data.result
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
    proxy.$modal
        .confirm('是否确认导出所有待到货数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportAllInfo()
        })
}

/**
 * 修改按钮操作 
 * @param {行数据} row 
 */
function handleUpdate(row) {
    proxy.$modal
        .confirm('是否确认到货?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(() => {
            confirmArrive(row.asnId).then(() => {
                proxy.$modal.msgSuccess("确认成功！")
                getList()
            })
        })
}

getList()
</script>

<style></style>