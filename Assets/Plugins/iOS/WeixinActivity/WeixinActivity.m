#import "WeixinActivity.h"
@implementation WeixinSessionActivity

- (UIImage *)activityImage
{
    return [[[UIDevice currentDevice] systemVersion] intValue] >= 8 ? [UIImage imageNamed:@"WeixinResources/icon_session-8.png"] : [UIImage imageNamed:@"WeixinResources/icon_session.png"];
}

- (NSString *)activityTitle
{
    return NSLocalizedString(@"微信(WeChat)", nil);
}

@end


@implementation WeixinTimelineActivity

- (id)init
{
    self = [super init];
    if (self) {
        scene = WXSceneTimeline;
    }
    return self;
}

- (UIImage *)activityImage
{
    return [[[UIDevice currentDevice] systemVersion] intValue] >= 8 ? [UIImage imageNamed:@"WeixinResources/icon_timeline-8.png"] : [UIImage imageNamed:@"WeixinResources/icon_timeline.png"];
}

- (NSString *)activityTitle
{
    return NSLocalizedString(@"朋友圈(Timeline)", nil);
}


@end