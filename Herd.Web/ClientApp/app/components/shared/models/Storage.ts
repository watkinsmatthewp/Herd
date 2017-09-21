export abstract class StorageService {
    abstract getItem(key: string): any;
    abstract setItem(key: string, val: any): void;
    abstract removeItem(key: string): void;
}

export class BrowserStorage extends StorageService {
    getItem(key: string) {
        return localStorage.getItem(key);
    }

    setItem(key: string, val: any) {
        localStorage.setItem(key, val);
    }
    removeItem(key: string) {
        localStorage.removeItem(key);
    }
}

export class ServerStorage extends StorageService {
    getItem(key: string) {
        return '';
    }

    setItem(key: string, val: any) {}

    removeItem(key: string) {}
}