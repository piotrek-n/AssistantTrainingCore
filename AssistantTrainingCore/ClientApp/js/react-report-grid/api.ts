import axios from 'axios';
import { handleApiSuccess, handleApiError, IApiResult } from '../shared/api';
import { IncompleteTrainingDataReport, WorkersWithoutTrainingResult, InstructionsWithoutTrainingResult, Product } from './models';


const productsApiUrl = '/api/Products';

export interface ResponseObject<T> {
    Data: T[];
    Total: number;
    AggregateResults?: any;
    Errors?: any;
}

export const getProducts = (queryStr: string, value:number): Promise<ResponseObject<Product>> => {
    return axios.get<ResponseObject<Product>>(`${productsApiUrl}?${queryStr}&value=${value}`)
    .then(response => response.data);
};

export const addNote = (product: Product): Promise<IApiResult> => {
    return axios.post(productsApiUrl, product)
    .then(handleApiSuccess)
    .catch(handleApiError);
}

export const deleteNote = (id: string): Promise<IApiResult> => {
    return axios.delete(`${productsApiUrl}/${id}`)
    .then(handleApiSuccess)
    .catch(handleApiError);
}

export const getIncompleteTraining= (queryStr: string, value: number): Promise<ResponseObject<IncompleteTrainingDataReport>> => {
    return axios.get<ResponseObject<IncompleteTrainingDataReport>>(`${productsApiUrl}?${queryStr}&value=${value}`)
        .then(response => response.data);
};

export const getWorkersWithoutTraining = (queryStr: string, value: number): Promise<ResponseObject<WorkersWithoutTrainingResult>> => {
    return axios.get<ResponseObject<WorkersWithoutTrainingResult>>(`${productsApiUrl}?${queryStr}&value=${value}`)
        .then(response => response.data);
};

export const getInstructionsWithoutTraining = (queryStr: string, value: number): Promise<ResponseObject<InstructionsWithoutTrainingResult>> => {
    return axios.get<ResponseObject<InstructionsWithoutTrainingResult>>(`${productsApiUrl}?${queryStr}&value=${value}`)
        .then(response => response.data);
};