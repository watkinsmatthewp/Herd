import { Account } from './Account';
import { Relationship } from './Relationship';

export class UserCard {
    MastodonUserId: number;
    MastodonUserName: string;
    MastodonDisplayName: string;
    MastodonProfileImageURL: string;
    MastodonHeaderImageUrl: string;
    MastodonShortBio: string;
    FollowsCurrentUser: boolean;
    CurrentUserIsFollowing: boolean;
}