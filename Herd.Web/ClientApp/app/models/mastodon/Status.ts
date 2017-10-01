import { Account } from './Account';

export class Status {
    Account: Account;
    Content: string;
    CreatedAt: Date;
    Favourited: boolean;
    FavouritesCount: number;
    Id: number;
    InReplyToAccountId: number;
    InReplyToId: number;
    //IEnumerable < Attachment > MediaAttachments ;
    //IEnumerable < Mention > Mentions;
    Reblog: Status;
    ReblogCount: number;
    Reblogged: boolean;
    Sensitive: boolean;
    SpoilerText: string;
    //IEnumerable < Tag > Tags;
    Uri: string;
    Url: string;
    //Mastonet.Visibility Visibility;
}