#import <UIKit/UIKit.h>
#import "WXApi.h"

@interface WeixinActivityBase : UIActivity {
    NSString *title;
    UIImage *image;
    NSURL *url;
    enum WXScene scene;
}

- (void)setThumbImage:(SendMessageToWXReq *)req;

@end
