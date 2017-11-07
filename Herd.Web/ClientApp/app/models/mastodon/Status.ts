import { Account } from './Account';
import { Visibility } from './Visibility';

export class Status {
    Author: Account;
    Content: string;
    CreatedOnUTC: Date;
    FavouritesCount: number;
    Id: string;
    InReplyToPostId: string;
    IsFavourited: boolean;
    IsReblogged: boolean;
    IsSensitive: boolean;
    MediaAttachment: string;
    ReblogCount: number;
    SpoilerText: string;
    Visibility: Visibility;

    // Context
    Ancestors: Status[];
    Descendants: Status[];
}