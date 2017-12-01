import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'updateHtmlLinks'
})
export class UpdateHtmlLinksPipe implements PipeTransform {

    // Update these base urls if they change
    profileBase = "/profile/";
    searchBase = "/searchresults?searchString=";

    constructor() { }

    /**
     * Replaces the href attributes from mastodon to use Herds links instead.
     * NOTE: if the urls are changed in app.module.shared then the profile/searchResults will have to be set as well.
     *
     * Example:
     *
     *     MENTION WITH '@'
     *     IN: <p><span class="h-card"><a href="https://mastodon.social/@jcstone3" class="u-url mention">@<span>jcstone3</span></a></span> hows it going?</p>
     *    OUT: <p><span class="h-card"><a [routerLink]="['/profile', userID]" class="u-url mention">@<span>jcstone3</span></a></span> hows it going?</p>
     *
     *
     *     HASHTAGS WITH '#'
     *     IN: <p>
     *              <a href="https://mastodon.social/tags/mastodon" class="mention hashtag" rel="tag">#<span>Mastodon</span></a>
     *              <a href="https://mastodon.social/tags/patreon" class="mention hashtag" rel="tag">#<span>Patreon</span></a>
     *              Update and retrospective
     *              <a href="https://www.patreon.com/posts/15635446" rel="nofollow noopener" target="_blank">
     *                  <span class="invisible">https://www.</span><span class="">patreon.com/posts/15635446</span><span class="invisible"></span>
     *              </a>
     *          </p>
     *    OUT: <p>
     *              <a [routerLink]="['/searchresults']" [queryParams]="{ searchString: hashtag.Name }" class="mention hashtag" rel="tag">#<span>Mastodon</span></a>
     *              <a [routerLink]="['/searchresults']" [queryParams]="{ searchString: hashtag.Name }" class="mention hashtag" rel="tag">#<span>Patreon</span></a>
     *              Update and retrospective
     *              <a href="https://www.patreon.com/posts/15635446" rel="nofollow noopener" target="_blank">
     *                  <span class="invisible">https://www.</span><span class="">patreon.com/posts/15635446</span><span class="invisible"></span>
     *              </a>
     *          </p>
     *
     * @param html
     */
    transform(html: string) {
        var element = document.createElement('div');
        element.innerHTML = html;

        // Get all anchors
        var anchors = element.getElementsByTagName("a");

        // Iterate through them
        for (let i = 0; i < anchors.length; i++) {
            let el = anchors[i];

            // username link
            if (el.className.indexOf("u-url") >= 0) {
                let profileID = el.innerText;
                el.setAttribute('href', this.profileBase + profileID);
            }

            // hashtag link
            if (el.className.indexOf("hashtag") >= 0) {
                let hashtag = el.getElementsByTagName("span")[0].innerText;
                el.setAttribute('href', this.searchBase + hashtag);
            }
        }
        return element.outerHTML;
    }

}