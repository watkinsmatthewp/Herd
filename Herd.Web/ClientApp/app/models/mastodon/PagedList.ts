class PageInformation {
    EarlierPageMaxID: string;
    NewerPageSinceID: string;
}

export class PagedList<T> {
    PageInformation: PageInformation;
    Items: T[] = [];
}