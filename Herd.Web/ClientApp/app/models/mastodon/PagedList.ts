class PagingOptions {
    MaxID: string;
    SinceID: string;
    Limit: number = 30;
}

export class PagedList<T> {
    PagingInformation: PagingOptions;
    Items: T[];
}