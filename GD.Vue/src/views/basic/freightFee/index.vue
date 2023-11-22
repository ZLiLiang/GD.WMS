<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="承运商" prop="carrier">
                <el-input v-model="queryParams.carrier" placeholder="请输入承运商" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="始发城市" prop="departureCity">
                <el-input v-model="queryParams.departureCity" placeholder="请输入始发城市" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="到货城市" prop="arrivalCity">
                <el-input v-model="queryParams.arrivalCity" placeholder="请输入到货城市" clearable style="width: 160px" />
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
                <el-button type="success" plain icon="upload" @click="handleImport">导入</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="loading" :data="freightFeeList" highlight-current-row>
            <el-table-column label="承运商" prop="carrier" v-if="columns.showColumn('carrier')" />
            <el-table-column label="始发城市" prop="departureCity" v-if="columns.showColumn('departureCity')" />
            <el-table-column label="到货城市" prop="arrivalCity" v-if="columns.showColumn('arrivalCity')" />
            <el-table-column label="单公斤运费" prop="pricePerWeight" v-if="columns.showColumn('pricePerWeight')" />
            <el-table-column label="单立方米运费" prop="pricePerVolume" v-if="columns.showColumn('pricePerVolume')" />
            <el-table-column label="最小运费" prop="minPayment" v-if="columns.showColumn('minPayment')" />
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
        <el-dialog :title="title" v-model="open" width="500px" :show-close="false" draggable="true">
            <el-form :model="form" :rules="rules" ref="freightFeeRef" label-width="80px">
                <el-form-item label="承运商" prop="carrier">
                    <el-input v-model="form.carrier" placeholder="请输入承运商" />
                </el-form-item>
                <el-form-item label="始发城市" prop="departureCity">
                    <el-input v-model="form.departureCity" placeholder="请输入始发城市" />
                </el-form-item>
                <el-form-item label="到货城市" prop="arrivalCity">
                    <el-input v-model="form.arrivalCity" placeholder="请输入到货城市" />
                </el-form-item>
                <el-form-item label="单公斤运费" prop="pricePerWeight">
                    <el-input v-model="form.pricePerWeight" placeholder="请输入单公斤运费" />
                </el-form-item>
                <el-form-item label="单立方米运费" prop="pricePerVolume">
                    <el-input v-model="form.pricePerVolume" placeholder="请输入单立方米运费" />
                </el-form-item>
                <el-form-item label="最小运费" prop="minPayment">
                    <el-input v-model="form.minPayment" placeholder="请输入最小运费" />
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

        <!-- 导入操作对话框 -->
        <el-dialog :title="upload.title" v-model="upload.open" width="500px" :show-close="false" :draggable="true">
            <el-upload name="file" ref="uploadRef" :limit="1" accept=".xlsx,.xls" :headers="upload.headers"
                :action="upload.url" :on-progress="handleFileUploadProgress" :on-success="handleFileSuccess"
                :auto-upload="false" drag>
                <el-icon class="el-icon--upload">
                    <upload-filled />
                </el-icon>
                <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
                <template #tip>
                    <div class="el-upload__tip text-center">
                        <span>仅允许导入xls、xlsx格式文件。</span>
                        <el-link type="primary" :underline="false" style="font-size: 12px; vertical-align: baseline"
                            @click="importTemplate">下载模板</el-link>
                    </div>
                </template>
            </el-upload>

            <template #footer>
                <el-button @click="upload.open = false">取消</el-button>
                <el-button type="primary" @click="submitFileForm">提交</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script setup>
import { getToken } from "@/utils/auth";
import { getAllInfo, addInfo, editInfo, getInfo, deleteInfo, exportAllInfo, exportTemplate } from "@/api/warehousManagement/freightFee"

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 供应商列表
const freightFeeList = ref([])
// 展示对话框
const open = ref(false)
// 对话框标题
const title = ref('')
// 时间范围
const dateRange = ref([])
// 用户导入参数
const upload = reactive({
    // 是否显示弹出层（用户导入）
    open: false,
    // 弹出层标题（用户导入）
    title: "",
    // 是否禁用上传
    isUploading: false,
    // 设置上传的请求头部
    headers: { Authorization: "Bearer " + getToken() },
    // 上传的地址
    url: import.meta.env.VITE_APP_BASE_API + "/warehousemanagement/freightFee/importData",
});
// 数据
const data = reactive({
    form: {
        carrier: undefined,
        departureCity: undefined,
        arrivalCity: undefined,
        pricePerWeight: undefined,
        pricePerVolume: undefined,
        minPayment: undefined,
        isValid: undefined
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        carrier: undefined,
        departureCity: undefined,
        arrivalCity: undefined
    },
    rules: {
        carrier: [{ required: true, message: '承运商不能为空', trigger: 'blur' }],
        departureCity: [{ required: true, message: '始发城市不能为空', trigger: 'blur' }],
        arrivalCity: [{ required: true, message: '到货城市不能为空', trigger: 'blur' }],
        pricePerWeight: [{ required: true, message: '单公斤运费不能为空且只能为数字', trigger: 'blur', pattern: /^\d+$|^\d*\.\d+$/ }],
        pricePerVolume: [{ required: true, message: '单立方米运费不能为空且只能为数字', trigger: 'blur', pattern: /^\d+$|^\d*\.\d+$/ }],
        minPayment: [{ required: true, message: '最小运费不能为空且只能为数字', trigger: 'blur', pattern: /^\d+$|^\d*\.\d+$/ }],
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `承运商`, visible: true, prop: 'carrier' },
    { key: 1, label: `始发城市`, visible: true, prop: 'departureCity' },
    { key: 2, label: `到货城市`, visible: true, prop: 'arrivalCity' },
    { key: 3, label: `单公斤运费`, visible: true, prop: 'pricePerWeight' },
    { key: 4, label: `单立方米运费`, visible: true, prop: 'pricePerVolume' },
    { key: 5, label: `最小运费`, visible: true, prop: 'minPayment' },
    { key: 6, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 7, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 8, label: `是否有效`, visible: true, prop: 'isValid' },
    { key: 9, label: `操作`, visible: true, prop: 'operate' }
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
        carrier: undefined,
        departureCity: undefined,
        arrivalCity: undefined,
        pricePerWeight: undefined,
        pricePerVolume: undefined,
        minPayment: undefined,
        isValid: undefined
    }
    proxy.resetForm('freightFeeRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        freightFeeList.value = res.data.result
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
 * 新增按钮操作
 */
function handleAdd() {
    reset()
    open.value = true
    title.value = "添加运费信息"
}

/**
 * 导入按钮操作
 */
function handleImport() {
    upload.title = "运费信息导入";
    upload.open = true;
}

/**
 * 导出按钮操作 
 */
function handleExport() {
    proxy
        .$modal
        .confirm('是否确认导出所有运费信息数据项?', '警告', {
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
    proxy.$refs['freightFeeRef'].validate((valid) => {
        if (valid) {
            if (form.value.freightFeeId != undefined) {
                editInfo(form.value.freightFeeId, form.value).then(res => {
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
    const freightFeeId = row.freightFeeId
    getInfo(freightFeeId).then(response => {
        form.value = response.data
        open.value = true
        title.value = '修改运费信息'
    })
    open.value = true
}

/**
 * 删除按钮操作
 * @param {行数据} row 
 */
function handleDelete(row) {
    const freightFeeId = row.freightFeeId
    proxy
        .$modal
        .confirm('是否确认删除"' + row.carrier + '"的数据项？')
        .then(() => {
            return deleteInfo(freightFeeId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
        .catch(() => { })
}

/**
 * 文件上传中处理
 * @param {事件} event
 * @param {文件} file
 * @param {文件列表} fileList
 */
function handleFileUploadProgress(event, file, fileList) {
    upload.isUploading = true;
}

/**
 * 文件上传成功处理
 * @param {响应内容} response
 * @param {文件} file
 * @param {文件列表} fileList
 */
function handleFileSuccess(response, file, fileList) {
    const { code, msg, data } = response;
    upload.open = false;
    upload.isUploading = false;
    proxy.$refs["uploadRef"].clearFiles();
    proxy
        .$modal
        .confirm('导入结果:' + data.item1)
        .then(() => {
            getList();
        })
}

/**
 * 下载模板操作
 */
async function importTemplate() {
    await exportTemplate()
}

/**
 * 提交上传文件
 */
function submitFileForm() {
    proxy.$refs["uploadRef"].submit();
}

getList()
</script>

<style></style>