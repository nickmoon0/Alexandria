package alexandria.services;

import alexandria.dtos.BaseDto;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.Arrays;
import java.util.Map;

public class JsonService {

    private static final Logger logger = LoggerFactory.getLogger(JsonService.class);

    public <T extends BaseDto> String createJson(String representation, String id, Class<T> valueType)
    {
        try {
            // Parse JSON string to a Map
            ObjectMapper objectMapper = new ObjectMapper();
            T map = objectMapper.readValue(representation, valueType);

            if (id != null) {
                map.id = id;
            }

            // Convert the Map back to a JSON string
            return objectMapper.writeValueAsString(map);
        } catch (Exception e) {
            logger.error(Arrays.toString(e.getStackTrace()));
            return null;
        }
    }
}
