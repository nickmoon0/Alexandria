package alexandria.dtos;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class UserDto extends BaseDto{
    public String firstName;
    public String lastName;
    public String email;
    public String username;
}
