import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'updateHtmlLinks'
})
export class UpdateHtmlLinksPipe implements PipeTransform {

    constructor() { }

    /**
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
        let atIndices = [];
        let hashIndices = [];

        for (let i = 0; i < html.length; i++) {
            if (html[i] === "@") atIndices.push(i);
            if (html[i] === "#") hashIndices.push(i);
        }

        if (atIndices.length > 0) {
            for (let i = 0; i < atIndices.length; i++) {
                let indexOfAt = atIndices[i];
                
                let indexOfNextSpace = html.indexOf(" ", indexOfAt);
                let subStr = html.substring(indexOfAt, indexOfNextSpace);
            }

            
        }

        if (hashIndices.length > 0) {
            for (let i = 0; i < hashIndices.length; i++) {
                let indexOfHash = hashIndices[i];
            }
        }

        return "";
    }

}