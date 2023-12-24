import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/operation/freeze'

/**
 * 分页查询所有数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getAllInfo(query) {
    return request({
        url: baseUrl,
        method: 'get',
        params: query
    })
}

/**
 * 通过id查询单个冻结/解冻信息
 * @param {冻结/解冻信息ID} id 
 * @returns 封装请求
 */
export function getInfo(id) {
    let url = `${baseUrl}/get/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'get'
    })
}

/**
 * 新增冻结/解冻信息
 * @param {冻结/解冻信息实体} entity 
 * @returns 封装请求
 */
export function addInfo(entity) {
    let url = `${baseUrl}/add`
    return request({
        url: url,
        method: 'post',
        data: entity
    })
}

/**
 * 导出仓库冻结/解冻
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/freezeExport`
    await downFile(url)
}