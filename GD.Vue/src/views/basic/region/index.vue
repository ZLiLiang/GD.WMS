<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="仓库名称" prop="warehouseName">
                <el-input v-model="queryParams.warehouseName" placeholder="请输入供仓库名称" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="库区名称" prop="regionName">
                <el-input v-model="queryParams.regionName" placeholder="请输入库区名称" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="库区类型" prop="regionProperty">
                <el-select v-model="queryParams.regionProperty" placeholder="选择库区类型" style="width: 160px">
                    <el-option v-for="(item, index) in regionPropertyOptions" :key="index" :label="item" :value="index" />
                </el-select>
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
                <el-button type="primary" plain icon="plus" @click="handleAdd">新增</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="loading" :data="regionList" highlight-current-row>
            <el-table-column label="仓库名称" prop="warehouseName" v-if="columns.showColumn('warehouseName')" />
            <el-table-column label="库区名称" prop="regionName" v-if="columns.showColumn('regionName')" />
            <el-table-column label="库区类型" prop="regionProperty" v-if="columns.showColumn('regionProperty')">
                <template #default="scope">
                    <span>{{ regionPropertyOptions[scope.row.regionProperty] }}</span>
                </template>
            </el-table-column>
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />
            <el-table-column label="是否有效" prop="isValid" v-if="columns.showColumn('isValid')">
                <template #default="scope">
                    <el-tag v-if="scope.row.isValid === 1" type="success">是</el-tag>
                    <el-tag v-else type="danger">否</el-tag>
                </template>
            </el-table-column>

            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <el-button text icon="edit" title="编辑" @click.stop="handleUpdate(scope.row)">
                        </el-button>
                        <el-button text icon="delete" title="删除" @click.stop="handleDelete(scope.row)">
                        </el-button>
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 对话框 -->
        <el-dialog :title="title" v-model="open" width="350px" :show-close="false" :draggable="true">
            <el-form :model="form" :rules="rules" ref="regionRef" label-width="80px">
                <el-form-item label="仓库名称" prop="warehouseId">
                    <el-select v-model="form.warehouseId" placeholder="请选择仓库">
                        <el-option v-for="item in warehouseOptions" :key="item.warehouseId" :label="item.warehouseName"
                            :value="item.warehouseId" style="width: 200px;" />
                    </el-select>
                </el-form-item>
                <el-form-item label="库区名称" prop="regionName">
                    <el-input v-model="form.regionName" placeholder="请输入库区名称" style="width: 218.4px;" />
                </el-form-item>
                <el-form-item label="库区类型" prop="regionProperty">
                    <el-select v-model="form.regionProperty" placeholder="选择库区类型">
                        <el-option v-for="(item, index) in regionPropertyOptions" :key="index" :label="item" :value="index"
                            style="width: 200px;" />
                    </el-select>
                </el-form-item>
                <el-form-item label="是否有效" prop="isValid">
                    <el-switch v-model="form.isValid" :active-value="1" :inactive-value="0" />
                </el-form-item>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>

    </div>
</template>

<script setup>
import { getAllInfo, getOptions, addInfo, editInfo, deleteInfo, getInfo, exportAllInfo } from '@/api/warehousManagement/region';

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 库区列表
const regionList = ref([])
// 库区类型
const regionPropertyOptions = ["拣货区", "备货区", "收货区", "退货区", "次品区", "存货区"]
//仓库选项列表
const warehouseOptions = ref([])
// 展示对话框
const open = ref(false)
// 对话框标题
const title = ref('')
// 时间范围
const dateRange = ref([])
// 数据
const data = reactive({
    form: {},
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        warehouseName: undefined,
        regionName: undefined,
        regionProperty: undefined
    },
    rules: {
        warehouseId: [{ required: true, message: '仓库名称不能为空', trigger: 'blur' }],
        regionName: [{ required: true, message: '库区名称不能为空', trigger: 'blur' }],
        regionProperty: [{ required: true, message: '库区类型不能为空', trigger: 'blur' }]
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `仓库名称`, visible: true, prop: 'warehouseName' },
    { key: 1, label: `库区名称`, visible: true, prop: 'regionName' },
    { key: 2, label: `库区类型`, visible: true, prop: 'regionProperty' },
    { key: 3, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 4, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 5, label: `是否有效`, visible: true, prop: 'isValid' },
    { key: 6, label: `操作`, visible: true, prop: 'operate' }
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
        warehouseId: undefined,
        regionName: undefined,
        regionProperty: undefined,
        isValid: undefined
    }
    proxy.resetForm('regionRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        regionList.value = res.data.result
        total.value = res.data.totalNum
    })
    getOptions().then(res => {
        warehouseOptions.value = res.data
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
 * 新增按钮操作
 */
function handleAdd() {
    reset()
    open.value = true
    title.value = "新增库区信息"
}

/**
 * 导出按钮操作 
 */
function handleExport() {
    proxy
        .$modal
        .confirm('是否确认导出所有库区信息数据项?', '警告', {
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
    proxy.$refs['regionRef'].validate((valid) => {
        if (valid) {
            if (form.value.regionId != undefined) {
                editInfo(form.value.regionId, form.value).then(res => {
                    proxy.$modal.msgSuccess('修改成功')
                    open.value = false
                    getList()
                })
            } else {
                addInfo(form.value).then(res => {
                    proxy.$modal.msgSuccess('新增成功')
                    open.value = false
                    getList()
                })
            }
        }
    })
}

/**
 * 取消按钮
 */
function cancel() {
    open.value = false
    reset()
}

/**
 * 修改按钮操作 
 * @param {行数据} row 
 */
function handleUpdate(row) {
    reset()
    const regionId = row.regionId
    getInfo(regionId).then(response => {
        form.value = response.data
        open.value = true
        title.value = '修改库区信息'
    })
    open.value = true
}

/**
 * 删除按钮操作
 * @param {行数据} row 
 */
function handleDelete(row) {
    const regionId = row.regionId
    proxy
        .$modal
        .confirm('是否确认删除"' + row.regionName + '"的数据项？')
        .then(() => {
            return deleteInfo(regionId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
        .catch(() => { })
}

getList()
</script>

<style></style>