package alexandria.services;

import alexandria.dtos.BaseDto;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.Arrays;

public class MessageService {

    private static final Logger logger = LoggerFactory.getLogger(MessageService.class);

    public <T extends BaseDto> T mapObject(String representation, String id, Class<T> valueType)
    {
        try {
            // Parse JSON string to a Map
            ObjectMapper objectMapper = new ObjectMapper();
            T map = objectMapper.readValue(representation, valueType);

            if (id != null) {
                map.id = id;
            }

            // Convert the Map back to a JSON string
            return map;
        } catch (Exception e) {
            logger.error(Arrays.toString(e.getStackTrace()));
            return null;
        }
    }

    public <T> String createJson(T obj) {
        // Create ObjectMapper instance
        ObjectMapper objectMapper = new ObjectMapper();

        // Serialize the object into JSON
        try {
            return objectMapper.writeValueAsString(obj);
        } catch (JsonProcessingException e) {
            logger.error("Failed to convert object to json");
            return null;
        }
    }
}
