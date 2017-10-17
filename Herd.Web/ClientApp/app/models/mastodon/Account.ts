import { Relationship } from './Relationship';

export class Account {
    FollowersCount: number;
    FollowingCount: number;
    FollowsActiveUser: boolean;
    IsFollowedByActiveUser: boolean;
    MastodonDisplayName: string;
    MastodonHeaderImageUrl: string;
    MastodonProfileImageURL: string;
    MastodonShortBio: string;
    MastodonUserId: number;
    MastodonUserName: string;
    PostCount: number;
}