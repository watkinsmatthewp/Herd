import { Account } from './Account';
import { Status } from './Status';

export class MastodonNotification {
    Id: number;
    Type: string;
    CreatedAt: Date;
    Account: Account;
    Status: Status;
}