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
                <el-button type="primary" plain icon="lock" @click="handleLock">冻结</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="success" plain icon="unlock" @click="handleUnlock">解冻</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="loading" :data="freezeList" highlight-current-row>
            <el-table-column label="作业单号" prop="jobCode" />
            <el-table-column label="作业状态" prop="jobType">
                <template #default="scope">
                    <el-tag type="" effect="dark" v-if="scope.row.jobType === 0">冻结</el-tag>
                    <el-tag color="#626aef" effect="dark" v-else>解冻</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="所在仓库" prop="warehouseName" />
            <el-table-column label="所在库位" prop="locationCode" />
            <el-table-column label="商品编码" prop="spuCode" />
            <el-table-column label="商品名称" prop="spuName" />
            <el-table-column label="规格编码" prop="skuCode" />
            <el-table-column label="操作人" prop="handler" />
            <el-table-column label="操作时间" prop="handlerTime" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />

            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <el-button text icon="view" title="查看" @click.stop="handleDetail(scope.row)" />
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 对话框 -->
        <el-dialog :title="title" v-model="open" width="25%" :show-close="false">
            <el-form :model="form" :rules="rules" ref="freezeRef" label-width="80px">
                <el-form-item label="商品编码" prop="spuCode">
                    <el-input v-model="form.spuCode" placeholder="请输入商品编码" :readonly="true" @click.native="spuCodeClick" />
                </el-form-item>
                <el-form-item label="商品名称" prop="spuName">
                    <el-input v-model="form.spuName" placeholder="请输入商品名称" disabled style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="规格编码" prop="skuCode">
                    <el-input v-model="form.skuCode" placeholder="请输入规格编码" disabled style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="所在库位" prop="warehouseName">
                    <el-input v-model="form.warehouseName" placeholder="请输入所在库位" disabled style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="所在仓库" prop="locationCode">
                    <el-input v-model="form.locationCode" placeholder="请输入所在仓库" disabled style="pointer-events: none;" />
                </el-form-item>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm" v-show="!showDetail">提交</el-button>
            </template>
        </el-dialog>

        <!-- 库存选择 -->
        <z-StockSelectDialog v-model:visible="stockSelectOpen" v-model:sqlTitle="sqlTitle" @dialogData="dialogStockData" />
    </div>
</template>

<script setup>
import { getAllInfo, getInfo, addInfo, exportAllInfo } from "@/api/operation/freeze";

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 查看冻结/解冻详细信息
const showDetail = ref(false)
// 加载...
const loading = ref(true)
// 仓库冻结列表
const freezeList = ref([])
// 展示库存选择对话框
const stockSelectOpen = ref(false)
// 展示对话框
const open = ref(false)
// 对话框标题
const title = ref('')
// 时间范围
const dateRange = ref([])
// 库存查询条件
const sqlTitle = ref("")
// 数据
const data = reactive({
    form: {
        skuId: undefined,
        spuCode: undefined,
        spuName: undefined,
        skuCode: undefined,
        jobType: undefined,
        ownerId: undefined,
        locationId: undefined,
        warehouseName: undefined,
        locationCode: undefined
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        jobCode: undefined
    },
    rules: {
        spuCode: [{ required: true, message: '商品编码不能为空', trigger: 'blur' }],
        spuName: [{ required: true, message: '商品名称不能为空', trigger: 'blur' }],
        skuCode: [{ required: true, message: '规格编码不能为空', trigger: 'blur' }],
        warehouseName: [{ required: true, message: '所在库位不能为空', trigger: 'blur' }],
        locationCode: [{ required: true, message: '所在仓库不能为空', trigger: 'blur' }]
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 1, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 2, label: `操作`, visible: true, prop: 'operate' }
])
// 表单、搜索参数、规则
const { form, queryParams, rules } = toRefs(data)
// 当时实例
const { proxy } = getCurrentInstance()

/**
 * 重置操作表单
 */
function reset() {
    form.value = {
        skuId: undefined,
        spuCode: undefined,
        spuName: undefined,
        skuCode: undefined,
        jobType: undefined,
        ownerId: undefined,
        locationId: undefined,
        warehouseName: undefined,
        locationCode: undefined
    }
    proxy.resetForm('freezeRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        freezeList.value = res.data.result
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
 * 冻结按钮操作
 */
function handleLock() {
    reset()
    title.value = "库存冻结"
    open.value = true
    form.value.jobType = 0
    sqlTitle.value = ""
}

/**
 * 解冻按钮操作
 */
function handleUnlock() {
    reset()
    title.value = "库存解冻"
    open.value = true
    form.value.jobType = 1
    sqlTitle.value = "forzen"
}

/**
 * 导出按钮操作 
 */
function handleExport() {
    proxy.$modal
        .confirm('是否确认导出所有仓库冻结信息数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportAllInfo()
        })
}

/**
 * 提交按钮
 */
function submitForm() {
    proxy.$refs['freezeRef'].validate((valid) => {
        if (valid && form.value.skuId !== undefined && form.value.locationId !== undefined && form.value.ownerId !== undefined) {
            addInfo(form.value).then(res => {
                proxy.$modal.msgSuccess('新增成功')
                open.value = false
                getList()
            }).catch(res => {
                proxy.$modal.msgError(res.data)
            })
        }
    })
}

/**
 * 商品编码输入框点击函数
 */
function spuCodeClick() {
    if (showDetail.value) {
        return
    }
    form.value.skuId = undefined
    form.value.skuCode = undefined
    form.value.spuCode = undefined
    form.value.spuName = undefined
    form.value.ownerId = undefined
    form.value.locationId = undefined
    form.value.locationCode = undefined
    form.value.warehouseName = undefined

    stockSelectOpen.value = true
}

/**
 * 取消按钮
 */
function cancel() {
    open.value = false
    showDetail.value = false
    reset()
}

/**
 * 修改按钮操作 
 * @param {行数据} row 
 */
function handleDetail(row) {
    open.value = true
    showDetail.value = true
    getInfo(row.freezeId).then(res => {
        form.value = res.data
    })
}

/**
 * 库存选择框返回数据函数
 * @param {数据} val 
 */
function dialogStockData(val) {
    form.value.skuId = val.skuId
    form.value.skuCode = val.skuCode
    form.value.spuCode = val.spuCode
    form.value.spuName = val.spuName
    form.value.ownerId = val.ownerId
    form.value.locationId = val.locationId
    form.value.locationCode = val.locationCode
    form.value.warehouseName = val.warehouseName
}

getList()
</script>

<style></style>