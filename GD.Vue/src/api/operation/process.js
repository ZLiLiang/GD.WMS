import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/operation/process'

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
 * 通过id查询单个加工信息
 * @param {加工信息ID} id 
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
 * 新增加工信息
 * @param {加工信息实体} entity 
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
 * 根据id删除加工信息
 * @param {加工信息ID} id 
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
 * 根据id确认加工信息
 * @param {加工信息ID} id 
 * @returns 封装请求
 */
export function confirmProcess(id) {
    let url = `${baseUrl}/confirmProcess/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'put'
    })
}

/**
 * 根据id确认调整信息
 * @param {调整信息ID} id 
 * @returns 封装请求
 */
export function confirmAdjustment(id) {
    let url = `${baseUrl}/confirmAdjustment/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'put'
    })
}

/**
 * 导出仓库加工
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/processExport`
    await downFile(url)
}