package alexandria.eventHandlers.common;

public class EventHelpers {
    public static String getUserId(String resourcePath) {
        return resourcePath.substring(resourcePath.lastIndexOf("/") + 1);
    }
}
