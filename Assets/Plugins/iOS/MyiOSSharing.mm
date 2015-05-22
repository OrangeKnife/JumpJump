
#import <UIKit/UIKit.h>

#import "WeixinActivity/WeixinActivity.h"

@interface  ViewController : UIViewController
{
    NSArray *activity;
}


@end
// Method for image sharing
@implementation ViewController


-(void) shareMethod: (const char *) path : (const char *) shareMessage
{
    NSString *imagePath = [NSString stringWithUTF8String:path];
 
    //    UIImage *image      = [UIImage imageNamed:imagePath];
    UIImage *image = [UIImage imageWithContentsOfFile:imagePath];
    NSString *message   = [NSString stringWithUTF8String:shareMessage];
    NSArray *postItems  = @[message,image];
 
    if([WXApi isWXAppInstalled])
        activity = @[[[WeixinSessionActivity alloc] init], [[WeixinTimelineActivity alloc] init]];
    else
        activity = nil;
    
    UIActivityViewController *activityVc = [[UIActivityViewController alloc]initWithActivityItems:postItems applicationActivities:activity];
    
    
    
    activityVc.excludedActivityTypes = @[UIActivityTypeMessage,UIActivityTypeMail,UIActivityTypePrint,UIActivityTypeAssignToContact,UIActivityTypeCopyToPasteboard,UIActivityTypeAirDrop];
    
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    //[activityVc release];
}

//Method for only text sharing
-(void) shareOnlyTextMethod: (const char *) shareMessage
{
    
    NSString *message   = [NSString stringWithUTF8String:shareMessage];
    NSArray *postItems  = @[message];
    
    if([WXApi isWXAppInstalled])
        activity = @[[[WeixinSessionActivity alloc] init], [[WeixinTimelineActivity alloc] init]];
    else
        activity = nil;
    
    UIActivityViewController *activityVc = [[UIActivityViewController alloc] initWithActivityItems:postItems applicationActivities:activity];
    
    activityVc.excludedActivityTypes = @[UIActivityTypeMessage,UIActivityTypeMail,UIActivityTypePrint,UIActivityTypeAssignToContact,UIActivityTypeCopyToPasteboard,UIActivityTypeAirDrop];
    
    [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    //[activityVc release];
}

@end

extern "C"{
    
    void iOSRegisterWechat(const char* wechatId){
        NSString *wxid   = [NSString stringWithUTF8String:wechatId];

        [WXApi registerApp: wxid] ;
    }
}
 
// globally declare image sharing method
extern "C"{
    void iOSShareImgAndMessage(const char * path, const char * message){
        ViewController *vc = [[ViewController alloc] init];
        [vc shareMethod: path: message];
       // [vc release];
    }
}



 
 
// Globally declare text sharing method
extern "C"{
    void iOSShareMessage(const char * message){
        ViewController *vc = [[ViewController alloc] init];
        [vc shareOnlyTextMethod: message];
        //[vc release];
    }
}

