import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/receive/asn'

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
 * 获取供应商列表
 * @returns 供应商列表
 */
export function getSupplierOptions() {
    let url = `${baseUrl}/get/supplierOptions`
    return request({
        url: url,
        method: 'get'
    })
}

/**
 * 获取货主列表
 * @returns 货主列表
 */
export function getOnwerOptions() {
    let url = `${baseUrl}/get/ownerOptions`
    return request({
        url: url,
        method: 'get'
    })
}

/**
 * 通过id查询单个asn信息
 * @param {ans信息ID} id 
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
 * 新增asn信息
 * @param {asn信息实体} entity 
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
 * 根据asn信息id修改asn信息
 * @param {asn信息ID} id 
 * @param {asn信息实体} entity 
 * @returns 封装请求
 */
export function editInfo(id, entity) {
    let url = `${baseUrl}/edit/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'post',
        data: entity
    })
}

/**
 * 根据id删除asn信息
 * @param {asn信息ID} id 
 * @returns 封装请求
 */
export function deleteInfo(id) {
    let url = `${baseUrl}/delete/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'delete'
    })
}

/**
 * 导出asn信息
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/noticeExport`
    await downFile(url)
}