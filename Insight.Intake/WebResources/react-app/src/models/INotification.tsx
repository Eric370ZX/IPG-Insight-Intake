export interface INotification{
    message:string,
    sevirity:'success' | 'info' | 'warning' | 'error',
    autoHideDuration?:number
}