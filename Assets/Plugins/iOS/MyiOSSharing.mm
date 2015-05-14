
#import <UIKit/UIKit.h>

@interface  ViewController : UIViewController

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
 
    UIActivityViewController *activityVc = [[UIActivityViewController alloc]initWithActivityItems:postItems applicationActivities:nil];
    
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    //[activityVc release];
}

//Method for only text sharing
-(void) shareOnlyTextMethod: (const char *) shareMessage
{
    
    NSString *message   = [NSString stringWithUTF8String:shareMessage];
    NSArray *postItems  = @[message];
    
    UIActivityViewController *activityVc = [[UIActivityViewController alloc] initWithActivityItems:postItems applicationActivities:nil];
    
    // Check if IOS device is IOS8
    
    [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    //[activityVc release];
}

@end
 
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

