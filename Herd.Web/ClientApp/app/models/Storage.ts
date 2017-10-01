export abstract class Storage {
    abstract clear(): void;
    abstract getItem(key: string): any;
    abstract setItem(key: string, val: any): void;
    abstract removeItem(key: string): void;
}

export class BrowserStorage extends Storage {
    clear() {
        localStorage.clear();
    }

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

export class ServerStorage extends Storage {
    clear() {}
    getItem(key: string) { return ''; }
    setItem(key: string, val: any) {}
    removeItem(key: string) {}
}