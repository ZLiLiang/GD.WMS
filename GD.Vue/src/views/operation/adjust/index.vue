<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="作业单号" prop="jobCode">
                <el-input v-model="queryParams.jobCode" placeholder="请输入作业单号" clearable style="width: 160px" />
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
        <el-table v-loading="false" :data="adjustList" highlight-current-row>
            <el-table-column label="作业单号" prop="jobCode" />
            <el-table-column label="作业状态" prop="jobType">
                <template #default="scope">
                    <el-tag color="#C0C0C0" effect="dark" v-if="scope.row.jobType === 0">拆分</el-tag>
                    <el-tag color="#800000" effect="dark" v-else-if="scope.row.jobType === 1">组合</el-tag>
                    <el-tag color="#808000" effect="dark" v-else-if="scope.row.jobType === 2">盘点</el-tag>
                    <el-tag color="#008000" effect="dark" v-else-if="scope.row.jobType === 3">移动</el-tag>
                    <el-tag color="#008080" effect="dark" v-else>未知</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="商品编码" prop="spuCode" />
            <el-table-column label="商品名称" prop="spuName" />
            <el-table-column label="规格编码" prop="skuCode" />
            <el-table-column label="调整差异数量" prop="qty" />
            <el-table-column label="所在仓库" prop="warehouseName" />
            <el-table-column label="所在库位" prop="locationCode" />
            <!-- <el-table-column label="操作人" prop="handler" />
            <el-table-column label="操作时间" prop="handlerTime" /> -->
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />
    </div>
</template>

<script setup>
import { getAllInfo, exportAllInfo } from "@/api/operation/adjust";

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 仓库冻结列表
const adjustList = ref([])
// 时间范围
const dateRange = ref([])
// 数据
const data = reactive({
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        jobCode: undefined
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 1, label: `创建时间`, visible: false, prop: 'createTime' }
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
        adjustList.value = res.data.result
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
        .confirm('是否确认导出所有仓库调整信息数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportAllInfo()
        })
}

getList()
</script>

<style></style>