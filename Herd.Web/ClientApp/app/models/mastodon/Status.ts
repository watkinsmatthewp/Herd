import { Account } from './Account';
import { Visibility } from './Visibility';

export class Status {
    Author: Account;
    Content: string;
    CreatedOnUTC: Date;
    FavouritesCount: number;
    Id: number;
    InReplyToPostId: number;
    IsFavourited: boolean;
    IsReblogged: boolean;
    IsSensitive: boolean;
    ReblogCount: number;
    SpoilerText: string;
    Visibility: Visibility;

    // Context
    Ancestors: Status[];
    Descendants: Status[];
}